﻿using Fuzzlyn.Statics;
using Fuzzlyn.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Fuzzlyn.Methods;

internal class FuncBodyGenerator
{
    private readonly List<FuncGenerator> _funcs;
    private readonly Randomizer _random;
    private readonly TypeManager _types;
    private readonly StaticsManager _statics;
    private readonly Func<string> _genChecksumSiteId;
    private readonly int _funcIndex;
    private readonly FuzzType _returnType;
    private readonly bool _isInPrimaryClass;

    private readonly List<ScopeFrame> _scope = new();
    private int _finallyCount;
    private int _varCounter;
    private int _statementLevel = -1;

    public FuncBodyGenerator(
        List<FuncGenerator> funcs,
        Randomizer random,
        TypeManager types,
        StaticsManager statics,
        Func<string> genChecksumSiteId,
        int funcIndex,
        FuzzType returnType,
        bool isInPrimaryClass)
    {
        _funcs = funcs;
        _random = random;
        _types = types;
        _statics = statics;
        _genChecksumSiteId = genChecksumSiteId;
        _funcIndex = funcIndex;
        _returnType = returnType;
        _isInPrimaryClass = isInPrimaryClass;
    }

    private FuzzlynOptions Options => _random.Options;

    public BlockSyntax Block { get; private set; }
    public int NumStatements { get; private set; }
    // Represents the transitive call counts from this function to other functions.
    public Dictionary<int, long> CallCounts { get; } = new();

    public void Generate(List<ScopeValue> initialScope)
    {
        Block = GenBlock(initialScope, true);
    }

    private StatementSyntax GenStatement(bool allowReturn = true)
    {
        if (_finallyCount > 0)
            allowReturn = false;

        while (true)
        {
            StatementKind kind =
                (StatementKind)Options.StatementTypeDist.Sample(_random.Rng);

            if ((kind == StatementKind.Block || kind == StatementKind.If || kind == StatementKind.TryFinally || kind == StatementKind.Loop) &&
                ShouldRejectRecursion())
                continue;

            if (kind == StatementKind.Return && !allowReturn)
                continue;

            switch (kind)
            {
                case StatementKind.Block:
                    return GenBlock();
                case StatementKind.Assignment:
                    return GenAssignmentStatement();
                case StatementKind.Call:
                    return GenCallStatement(tryExisting: ShouldRejectRecursion());
                case StatementKind.If:
                    return GenIf();
                case StatementKind.TryFinally:
                    return GenTryFinally();
                case StatementKind.Return:
                    return GenReturn();
                case StatementKind.Loop:
                    return GenLoop();
                default:
                    throw new Exception("Unreachable");
            }
        }

        bool ShouldRejectRecursion()
            => Options.StatementRejection.Reject(_statementLevel, _random.Rng);
    }

    private BlockSyntax GenBlock(IEnumerable<ScopeValue> vars = null, bool root = false, int numStatements = -1)
    {
        if (numStatements == -1)
            numStatements = Options.BlockStatementCountDist.Sample(_random.Rng);

        _statementLevel++;

        ScopeFrame scope = new();
        _scope.Add(scope);

        if (vars != null)
            scope.Values.AddRange(vars);

        BlockSyntax block = Block(GenStatements());

        _scope.RemoveAt(_scope.Count - 1);

        _statementLevel--;

        return block;

        IEnumerable<StatementSyntax> GenStatements()
        {
            StatementSyntax retStmt = null;
            int numGenerated = 0;
            while (true)
            {
                StatementSyntax stmt = GenStatement(allowReturn: !root);

                if (stmt is ReturnStatementSyntax)
                {
                    retStmt = stmt;
                    break;
                }

                NumStatements++;
                yield return stmt;

                numGenerated++;
                if (numGenerated < numStatements)
                    continue;

                // For first block we ensure we get a minimum amount of statements
                if (root && _funcIndex == 0)
                {
                    if (_funcs.Sum(f => f.NumStatements) < Options.ProgramMinStatements)
                        continue;
                }

                break;
            }

            if (Options.EnableChecksumming)
            {
                foreach (StatementSyntax stmt in GenChecksumming(prefixRuntimeAccess: !_isInPrimaryClass, scope.Values, _genChecksumSiteId))
                    yield return stmt;
            }

            if (root && retStmt == null && _returnType != null)
                retStmt = GenReturn();

            if (retStmt != null)
            {
                NumStatements++;
                yield return retStmt;
            }
        }
    }

    private ExpressionSyntax AccessStatic(StaticField stat)
        => stat.CreateAccessor(prefixWithClass: !_isInPrimaryClass);

    private StatementSyntax GenAssignmentStatement()
    {
        LValueInfo lvalue = null;
        if (!_random.FlipCoin(Options.AssignToNewVarProb))
            lvalue = GenExistingLValue(null, int.MinValue);

        if (lvalue == null)
        {
            FuzzType newType = _types.PickType(Options.LocalIsByRefProb);
            // Determine if we should create a new local. We do this with a certain probabilty,
            // or always if the new type is a by-ref type (we cannot have static by-refs).
            if (newType is RefType || _random.FlipCoin(Options.NewVarIsLocalProb))
            {
                ScopeValue variable;
                string varName = $"var{_varCounter++}";
                ExpressionSyntax rhs;
                if (newType is RefType newRt)
                {
                    LValueInfo rhsLV = GenLValue(newRt.InnerType, int.MinValue);
                    variable = new ScopeValue(newType, IdentifierName(varName), rhsLV.RefEscapeScope, readOnly: false);
                    rhs = RefExpression(rhsLV.Expression);
                }
                else
                {
                    rhs = GenExpression(newType);
                    variable = new ScopeValue(newType, IdentifierName(varName), -(_scope.Count - 1), readOnly: false);
                }

                LocalDeclarationStatementSyntax decl =
                    LocalDeclarationStatement(
                        VariableDeclaration(
                            variable.Type.GenReferenceTo(),
                            SingletonSeparatedList(
                                VariableDeclarator(varName)
                                .WithInitializer(
                                    EqualsValueClause(rhs)))));

                _scope.Last().Values.Add(variable);

                return decl;
            }

            StaticField newStatic = _statics.GenerateNewField(newType);
            lvalue = new LValueInfo(AccessStatic(newStatic), newType, int.MaxValue, readOnly: false);
        }

        // Determine if we should generate a ref-reassignment. In that case we cannot do anything
        // clever, like generate compound assignments.
        FuzzType rhsType = lvalue.Type;
        if (lvalue.Type is RefType rt)
        {
            if (_random.FlipCoin(Options.AssignGenRefReassignProb))
            {
                RefExpressionSyntax refRhs = RefExpression(GenLValue(rt.InnerType, lvalue.RefEscapeScope).Expression);
                return
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            lvalue.Expression,
                            refRhs));
            }

            // We have a ref-type, but are not generating a ref-reassign, so lift the type and make a normal assignment.
            rhsType = rt.InnerType;
        }

        SyntaxKind assignmentKind = SyntaxKind.SimpleAssignmentExpression;
        // Determine if we should generate compound assignment.
        if (rhsType.AllowedAdditionalAssignmentKinds.Length > 0 && _random.FlipCoin(Options.CompoundAssignmentProb))
            assignmentKind = _random.NextElement(rhsType.AllowedAdditionalAssignmentKinds);

        // Early our for simple cases.
        if (assignmentKind == SyntaxKind.PreIncrementExpression ||
            assignmentKind == SyntaxKind.PreDecrementExpression)
        {
            return ExpressionStatement(PrefixUnaryExpression(assignmentKind, lvalue.Expression));
        }

        if (assignmentKind == SyntaxKind.PostIncrementExpression ||
            assignmentKind == SyntaxKind.PostDecrementExpression)
        {
            return ExpressionStatement(PostfixUnaryExpression(assignmentKind, lvalue.Expression));
        }

        // Right operand of shifts are always ints.
        if (assignmentKind == SyntaxKind.LeftShiftAssignmentExpression ||
            assignmentKind == SyntaxKind.RightShiftAssignmentExpression)
        {
            rhsType = _types.GetPrimitiveType(SyntaxKind.IntKeyword);
        }

        ExpressionSyntax right = GenExpression(rhsType);
        // For modulo and division we don't want to throw divide-by-zero exceptions,
        // so always or right-hand-side with 1.
        if (assignmentKind == SyntaxKind.ModuloAssignmentExpression ||
            assignmentKind == SyntaxKind.DivideAssignmentExpression)
        {
            right =
                CastExpression(
                    rhsType.GenReferenceTo(),
                    ParenthesizedExpression(
                        BinaryExpression(
                            SyntaxKind.BitwiseOrExpression,
                            ParenthesizeIfNecessary(right),
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(1)))));
        }

        return ExpressionStatement(AssignmentExpression(assignmentKind, lvalue.Expression, right));
    }

    private StatementSyntax GenCallStatement(bool tryExisting)
    {
        // If we are supposed to try existing first, then do not allow new
        ExpressionSyntax call = GenCall(null, allowNew: !tryExisting);

        while (call == null)
        {
            // There are no existing, so allow new until we get a new one
            call = GenCall(null, true);
        }

        return ExpressionStatement(call);
    }

    private StatementSyntax GenIf()
    {
        ExpressionSyntax guard;
        int attempts = 0;
        do
        {
            guard = GenExpression(new PrimitiveType(SyntaxKind.BoolKeyword));
        } while (guard is LiteralExpressionSyntax && attempts++ < 20);

        StatementSyntax gen;
        if (_random.FlipCoin(0.5))
        {
            gen = IfStatement(guard, GenBlock());
        }
        else
        {
            gen = IfStatement(guard, GenBlock(), ElseClause(GenBlock()));
        }
        return gen;
    }

    private StatementSyntax GenTryFinally()
    {
        int numStatements = Options.BlockStatementCountDist.Sample(_random.Rng);
        int tryStatements = _random.Next(numStatements);
        int finallyStatements = numStatements - tryStatements;

        BlockSyntax body = GenBlock(numStatements: tryStatements);
        _finallyCount++;
        BlockSyntax finallyBody = GenBlock(numStatements: finallyStatements);
        _finallyCount--;
        return
            TryStatement(
                body,
                List<CatchClauseSyntax>(),
                FinallyClause(finallyBody));
    }

    private StatementSyntax GenReturn()
    {
        if (_returnType == null)
            return ReturnStatement();

        ExpressionSyntax expr;
        if (_returnType is RefType rt)
            expr = RefExpression(GenLValue(rt.InnerType, 1).Expression);
        else
            expr = GenExpression(_returnType);

        return ReturnStatement(expr);
    }

    private StatementSyntax GenLoop()
    {
        string varName = $"var{_varCounter++}";
        ScopeValue indVar = new(_types.GetPrimitiveType(SyntaxKind.IntKeyword), IdentifierName(varName), -(_scope.Count - 1), true);

        VariableDeclarationSyntax decl =
            VariableDeclaration(
                indVar.Type.GenReferenceTo(),
                SingletonSeparatedList(
                    VariableDeclarator(varName)
                    .WithInitializer(
                        EqualsValueClause(
                            LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                Literal(0))))));

        ExpressionSyntax cond =
            BinaryExpression(
                SyntaxKind.LessThanExpression,
                IdentifierName(varName),
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(2)));

        ExpressionSyntax incr =
            PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, indVar.Expression);

        BlockSyntax block = GenBlock(new[] { indVar });

        ForStatementSyntax @for = ForStatement(decl, SeparatedList<ExpressionSyntax>(), cond, SingletonSeparatedList(incr), block);
        return @for;
    }

    private int _expressionLevel = -1;
    private ExpressionSyntax GenExpression(FuzzType type)
    {
        _expressionLevel++;

        Debug.Assert(!(type is RefType));
        ExpressionSyntax gen = null;
        do
        {
            ExpressionKind kind = (ExpressionKind)Options.ExpressionTypeDist.Sample(_random.Rng);
            switch (kind)
            {
                case ExpressionKind.MemberAccess:
                    gen = GenMemberAccess(type);
                    break;
                case ExpressionKind.Literal:
                    gen = GenLiteral(type);
                    break;
                case ExpressionKind.Unary:
                    if (AllowRecursion())
                        gen = GenUnary(type);
                    break;
                case ExpressionKind.Binary:
                    if (AllowRecursion())
                        gen = GenBinary(type);
                    break;
                case ExpressionKind.Call:
                    if (AllowRecursion())
                        gen = GenCall(type, true);
                    break;
                case ExpressionKind.Increment:
                    if (AllowRecursion())
                        gen = GenIncDec(type, true);
                    break;
                case ExpressionKind.Decrement:
                    if (AllowRecursion())
                        gen = GenIncDec(type, false);
                    break;
                case ExpressionKind.NewObject:
                    if (AllowRecursion())
                        gen = GenNewObject(type);
                    break;
                default:
                    throw new Exception("Unreachable");
            }
        }
        while (gen == null);

        bool AllowRecursion()
            => !Options.ExpressionRejection.Reject(_expressionLevel, _random.Rng);

        _expressionLevel--;

        return gen;
    }

    /// <summary>Returns an lvalue.</summary>
    private LValueInfo GenLValue(FuzzType type, int minRefEscapeScope)
    {
        Debug.Assert(type != null);

        LValueInfo lv = GenExistingLValue(type, minRefEscapeScope);

        if (lv == null)
        {
            StaticField newStatic = _statics.GenerateNewField(type);
            lv = new LValueInfo(AccessStatic(newStatic), type, int.MaxValue, readOnly: false);
        }

        return lv;
    }

    private LValueInfo GenExistingLValue(FuzzType type, int minRefEscapeScope)
    {
        Debug.Assert(type == null || !(type is RefType));

        LValueKind kind = (LValueKind)Options.ExistingLValueDist.Sample(_random.Rng);

        if (kind == LValueKind.RefReturningCall)
        {
            List<FuncGenerator> refReturningFuncs = new();
            foreach (FuncGenerator func in _funcs.Skip(_funcIndex + 1))
            {
                if (func.ReturnType is not RefType rt)
                    continue;

                if (type == null || type == rt.InnerType)
                    refReturningFuncs.Add(func);
            }

            // If we don't have a ref-returning function, fall back to local/static by retrying.
            // The reason we don't always fall back to trying other options is that otherwise
            // we may stack overflow in cases like ref int M(ref int a), if we have no locals
            // or statics of type int. We would keep generating method calls in this case.
            // This is kind of a hack, although it is pretty natural to pick statics/locals for
            // lvalues, so it is not that big of a deal.
            if (refReturningFuncs.Count == 0)
                return GenExistingLValue(type, minRefEscapeScope);

            FuncGenerator funcToCall = _random.NextElement(refReturningFuncs);

            ExpressionSyntax invocExpr = GenInvocFuncExpr(funcToCall);
            // When calling a func that returns a by-ref, we need to take into account that the C# compiler
            // performs 'escape-analysis' on by-refs. If we want to produce a non-local by-ref we are only
            // allowed to pass non-local by-refs. The following example illustrates the analysis performed
            // by the compiler:
            // ref int M(ref int b, ref int c) {
            //   int a = 2;
            //   return ref Max(ref a, ref b); // compiler error, returned by-ref could point to 'a' and escape
            //   return ref Max(ref b, ref c); // ok, returned ref cannot point to local
            // }
            // ref int Max(ref int a, ref int b) { ... }
            // This is the reason for the second arg passed to GenArgs. For example, if we want a call to return
            // a ref that may escape, we need a minimum ref escape scope of 1, since 0 could pass refs to locals,
            // and the result of that call would not be valid to return.
            ArgumentSyntax[] args = GenArgs(funcToCall, minRefEscapeScope, out int argsMinRefEscapeScope);
            InvocationExpressionSyntax invoc =
                InvocationExpression(
                    invocExpr,
                    ArgumentList(
                        SeparatedList(
                            args)));

            return new LValueInfo(invoc, ((RefType)funcToCall.ReturnType).InnerType, argsMinRefEscapeScope, readOnly: false);
        }

        List<LValueInfo> lvalues = CollectVariablePaths(type, minRefEscapeScope, kind == LValueKind.Local, kind == LValueKind.Static, requireAssignable: true);
        if (lvalues.Count == 0)
        {
            // We typically fall back to generating a static, so just try to find a static if we found no local.
            if (kind != LValueKind.Local)
                return null;

            lvalues = CollectVariablePaths(type, minRefEscapeScope, false, true, requireAssignable: true);
            if (lvalues.Count == 0)
                return null;
        }

        return _random.NextElement(lvalues);
    }

    /// <summary>
    /// Generates an access to a random member:
    /// * A static variable
    /// * A local variable
    /// * An argument
    /// * A field in one of these, if aggregate type
    /// * An element in one of these, if array type
    /// </summary>
    private ExpressionSyntax GenMemberAccess(FuzzType ft)
    {
        List<LValueInfo> paths =
            _random.FlipCoin(Options.MemberAccessSelectLocalProb)
            ? CollectVariablePaths(ft, int.MinValue, true, false, requireAssignable: false)
            : CollectVariablePaths(ft, int.MinValue, false, true, requireAssignable: false);

        return paths.Count > 0 ? _random.NextElement(paths).Expression : null;
    }

    /// <summary>
    /// Collect lvalues of the specified type.
    /// </summary>
    /// <param name="type">The type, or null to collect all lvalues.</param>
    /// <param name="minRefEscapeScope">The minimum scope that refs taken of the lvalue must be able to escape to.</param>
    /// <param name="collectLocals">Whether to collect locals.</param>
    /// <param name="collectStatics">Whether to collect statics.</param>
    /// <param name="requireAssignable">Whether the collected lvalues must be able to appear as the LHS of an assignment with an RHS of type <paramref name="type"/>.</param>
    private List<LValueInfo> CollectVariablePaths(FuzzType type, int minRefEscapeScope, bool collectLocals, bool collectStatics, bool requireAssignable)
    {
        List<LValueInfo> paths = new();

        if (collectLocals)
        {
            foreach (ScopeFrame sf in _scope)
            {
                foreach (ScopeValue variable in sf.Values)
                {
                    AppendVariablePaths(paths, variable);
                }
            }
        }

        if (collectStatics)
        {
            foreach (StaticField stat in _statics.Fields)
            {
                AppendVariablePaths(paths, new ScopeValue(stat.Type, AccessStatic(stat), int.MaxValue, false));
            }
        }

        Debug.Assert(type == null || !(type is RefType), "Cannot collect variables of ref type");

        // Verify that a type is allowed. Often we want to implicitly promote
        // a by-ref to its inner type, because we are taking a ref anyway or using
        // its value, and it is automatically promoted.
        // This checks for both of these cases:
        // ref int lhs = ...;
        // int rhs1 = ...;
        // ref int rhs2 = ...;
        // lhs = ref rhs1; // must be supported
        // lhs = ref rhs2; // must be supported
        bool IsAllowedType(FuzzType other)
        {
            if (type == null)
                return true;

            if (other is RefType rt)
                other = rt.InnerType; // Implicit deref

            if (other == type)
                return true;

            if (!requireAssignable)
            {
                // Allow implicit conversion from collected variable to type
                if (type is InterfaceType it && other is AggregateType agg && agg.Implements(it))
                    return true;
            }

            return false;
        }

        paths.RemoveAll(lv => lv.RefEscapeScope < minRefEscapeScope || (lv.ReadOnly && requireAssignable) || !IsAllowedType(lv.Type));
        return paths;
    }

    private ExpressionSyntax GenLiteral(FuzzType type)
    {
        return LiteralGenerator.GenLiteral(_types, _random, type);
    }

    private ExpressionSyntax GenUnary(FuzzType type)
    {
        if (type is not PrimitiveType pt)
            return null;

        if (pt.Keyword == SyntaxKind.BoolKeyword)
        {
            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, ParenthesizeIfNecessary(GenNonLiteralExpression(type)));
        }

        // Unary minus is not valid for ulong, so there we always pick ~
        SyntaxKind op =
            pt.Keyword == SyntaxKind.ULongKeyword
            ? SyntaxKind.BitwiseNotExpression
            : (SyntaxKind)Options.UnaryIntegralDist.Sample(_random.Rng);

        ExpressionSyntax expr = PrefixUnaryExpression(op, ParenthesizeIfNecessary(GenNonLiteralExpression(type)));

        UnOpTable table = op switch
        {
            SyntaxKind.UnaryPlusExpression => UnOpTable.UnaryPlus,
            SyntaxKind.UnaryMinusExpression => UnOpTable.UnaryMinus,
            SyntaxKind.BitwiseNotExpression => UnOpTable.BitwiseNot,
            _ => throw new Exception("Unexpected syntax kind sampled for unary integer op: " + op),
        };

        Debug.Assert(table.GetResultType(pt.Keyword).HasValue);
        if (table.GetResultType(pt.Keyword).Value != pt.Keyword)
            expr = CastExpression(type.GenReferenceTo(), ParenthesizedExpression(expr));

        return expr;
    }

    private ExpressionSyntax GenBinary(FuzzType type)
    {
        if (type is not PrimitiveType pt)
            return null;

        if (pt.Keyword == SyntaxKind.BoolKeyword)
            return GenBoolProducingBinary();

        return GenIntegralProducingBinary(pt);
    }

    private ExpressionSyntax GenBoolProducingBinary()
    {
        PrimitiveType leftType;
        PrimitiveType rightType;
        SyntaxKind op = (SyntaxKind)Options.BinaryBoolDist.Sample(_random.Rng);
        if (op == SyntaxKind.LogicalAndExpression || op == SyntaxKind.LogicalOrExpression ||
            op == SyntaxKind.ExclusiveOrExpression || op == SyntaxKind.BitwiseAndExpression ||
            op == SyntaxKind.BitwiseOrExpression)
        {
            // &&, ||, ^, &, | must be between bools to produce bool
            PrimitiveType boolType = _types.GetPrimitiveType(SyntaxKind.BoolKeyword);
            leftType = rightType = boolType;
        }
        else if (op == SyntaxKind.EqualsExpression || op == SyntaxKind.NotEqualsExpression)
        {
            leftType = _types.PickPrimitiveType(f => true);
            rightType = _types.PickPrimitiveType(f => BinOpTable.Equality.GetResultType(leftType.Keyword, f.Keyword).HasValue);
        }
        else
        {
            Debug.Assert(op == SyntaxKind.LessThanOrEqualExpression || op == SyntaxKind.LessThanExpression ||
                         op == SyntaxKind.GreaterThanOrEqualExpression || op == SyntaxKind.GreaterThanExpression);

            leftType = _types.PickPrimitiveType(f => f.Keyword != SyntaxKind.BoolKeyword);
            rightType = _types.PickPrimitiveType(f => BinOpTable.Relop.GetResultType(leftType.Keyword, f.Keyword).HasValue);
        }

        var (left, right) = GenLeftRightForBinary(leftType, rightType);

        return BinaryExpression(op, ParenthesizeIfNecessary(left), ParenthesizeIfNecessary(right));
    }

    private ExpressionSyntax ParenthesizeIfNecessary(ExpressionSyntax expr)
    {
        if (Helpers.RequiresParentheses(expr))
            return ParenthesizedExpression(expr);

        return expr;
    }

    private ExpressionSyntax GenIntegralProducingBinary(PrimitiveType type)
    {
        Debug.Assert(type.Info.IsIntegral);
        SyntaxKind op = (SyntaxKind)Options.BinaryIntegralDist.Sample(_random.Rng);

        BinOpTable table;
        if (op == SyntaxKind.LeftShiftExpression || op == SyntaxKind.RightShiftExpression)
            table = BinOpTable.Shifts;
        else
            table = BinOpTable.Arithmetic;

        PrimitiveType leftType = _types.PickPrimitiveType(f => f.Keyword != SyntaxKind.BoolKeyword);
        PrimitiveType rightType =
            _types.PickPrimitiveType(f => table.GetResultType(leftType.Keyword, f.Keyword).HasValue);

        var (left, right) = GenLeftRightForBinary(leftType, rightType);

        if (op == SyntaxKind.ModuloExpression || op == SyntaxKind.DivideExpression)
        {
            right = CastExpression(
                rightType.GenReferenceTo(),
                ParenthesizedExpression(
                    BinaryExpression(
                        SyntaxKind.BitwiseOrExpression,
                        ParenthesizeIfNecessary(right),
                        LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(1)))));
        }

        ExpressionSyntax expr = BinaryExpression(op, ParenthesizeIfNecessary(left), ParenthesizeIfNecessary(right));

        if (table.GetResultType(leftType.Keyword, rightType.Keyword) != type.Keyword)
            expr = CastExpression(type.GenReferenceTo(), ParenthesizedExpression(expr));

        return expr;
    }

    private (ExpressionSyntax, ExpressionSyntax) GenLeftRightForBinary(FuzzType leftType, FuzzType rightType)
    {
        ExpressionSyntax left = GenExpression(leftType);
        // There are two reasons we don't allow both left and right to be literals:
        // 1. If the computation overflows, this gives a C# compiler error
        // 2. The compiler is required to constant fold these expressions which is not interesting.
        if (left is LiteralExpressionSyntax)
            return (left, GenNonLiteralExpression(rightType));

        return (left, GenExpression(rightType));
    }

    private ExpressionSyntax GenNonLiteralExpression(FuzzType type)
    {
        ExpressionSyntax gen;
        do
        {
            gen = GenExpression(type);
        } while (gen is LiteralExpressionSyntax);

        return gen;
    }

    private ExpressionSyntax GenCall(FuzzType type, bool allowNew)
    {
        Debug.Assert(!(type is RefType), "Cannot GenCall to ref type -- use GenExistingLValue for that");

        FuncGenerator func;
        if (allowNew && _random.FlipCoin(Options.GenNewFunctionProb) && !Options.FuncGenRejection.Reject(_funcs.Count, _random.Rng))
        {
            type ??= _types.PickType(Options.ReturnTypeIsByRefProb);

            func = new FuncGenerator(_funcs, _random, _types, _statics, _genChecksumSiteId);
            func.Generate(type, true);
        }
        else
        {
            IEnumerable<FuncGenerator> funcs =
                _funcs
                .Skip(_funcIndex + 1)
                .Where(candidate =>
                {
                        // Make sure we do not get too many leaf calls. Here we compute what the new transitive
                        // number of calls would be to each function, and if it's too much, reject this candidate.
                        // Note that we will never reject calling a leaf function directly, even if the +1 puts
                        // us over the cap. That is intentional. We only want to limit the exponential growth
                        // which happens when functions call functions multiple times, and those functions also
                        // call functions multiple times.
                        foreach (var (transFunc, transNumCall) in candidate.CallCounts)
                    {
                        CallCounts.TryGetValue(transFunc, out long curNumCalls);
                        if (curNumCalls + transNumCall > Options.SingleFunctionMaxTotalCalls)
                            return false;
                    }

                    return true;
                });

            if (type != null)
                funcs = funcs.Where(f => f.ReturnType.IsCastableTo(type) || (f.ReturnType is RefType rt && rt.InnerType.IsCastableTo(type)));

            List<FuncGenerator> list = funcs.ToList();
            if (list.Count == 0)
                return null;

            func = _random.NextElement(list);
            type ??= func.ReturnType;
        }

        // Update transitive call counts before generating args, so we decrease chance of
        // calling the same methods in the arg expressions.
        CallCounts.TryGetValue(func.FuncIndex, out long numCalls);
        CallCounts[func.FuncIndex] = numCalls + 1;

        foreach (var (transFunc, transNumCalls) in func.CallCounts)
        {
            CallCounts.TryGetValue(transFunc, out long curNumCalls);
            CallCounts[transFunc] = curNumCalls + transNumCalls;
        }

        ExpressionSyntax invocFuncExpr = GenInvocFuncExpr(func);
        ArgumentSyntax[] args = GenArgs(func, 0, out _);
        InvocationExpressionSyntax invoc =
            InvocationExpression(
                invocFuncExpr,
                ArgumentList(
                    SeparatedList(args)));

        if (func.ReturnType == type || (func.ReturnType is AggregateType agg && type is InterfaceType it && agg.Implements(it)) || (func.ReturnType is RefType retRt && retRt.InnerType == type))
            return invoc;

        return CastExpression(type.GenReferenceTo(), invoc);
    }

    // Generates the expression that comes before the parentheses of the call.
    private ExpressionSyntax GenInvocFuncExpr(FuncGenerator func)
    {
        FuzzType funcThisType = (FuzzType)func.InstanceType ?? func.InterfaceType;
        if (_isInPrimaryClass && funcThisType == null)
        {
            // Both are in primary class, call directly without any class prefix
            return IdentifierName(func.Name);
        }

        if (funcThisType == null)
        {
            // Callee is in primary class, invoke it as static function with prefix
            return
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(CodeGenerator.ClassNameForStaticMethods),
                    IdentifierName(func.Name));
        }

        // Instance method, generate receiver
        ExpressionSyntax receiver = GenExpression(funcThisType);
        return
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                receiver,
                IdentifierName(func.Name));
    }

    private ArgumentSyntax[] GenArgs(FuncGenerator funcToCall, int minRefEscapeScope, out int argsMinRefEscapeScope)
    {
        ArgumentSyntax[] args = new ArgumentSyntax[funcToCall.Parameters.Length];
        argsMinRefEscapeScope = int.MaxValue;

        for (int i = 0; i < args.Length; i++)
        {
            FuzzType paramType = funcToCall.Parameters[i].Type;
            if (paramType is RefType rt)
            {
                LValueInfo lv = GenLValue(rt.InnerType, minRefEscapeScope);
                argsMinRefEscapeScope = Math.Min(argsMinRefEscapeScope, lv.RefEscapeScope);
                args[i] = Argument(lv.Expression).WithRefKindKeyword(Token(SyntaxKind.RefKeyword));
            }
            else
            {
                args[i] = Argument(GenExpression(paramType));
            }
        }

        return args;
    }

    private ExpressionSyntax GenIncDec(FuzzType type, bool isIncrement)
    {
        SyntaxKind[] acceptedTypes =
        {
                SyntaxKind.ULongKeyword,
                SyntaxKind.LongKeyword,
                SyntaxKind.UIntKeyword,
                SyntaxKind.IntKeyword,
                SyntaxKind.UShortKeyword,
                SyntaxKind.ShortKeyword,
                SyntaxKind.ByteKeyword,
                SyntaxKind.SByteKeyword,
            };

        if (type is not PrimitiveType pt || !acceptedTypes.Contains(pt.Keyword))
            return null;

        LValueInfo subject = GenExistingLValue(type, int.MinValue);
        if (subject == null)
            return null;

        ExpressionSyntax gen = PostfixUnaryExpression(
            isIncrement ? SyntaxKind.PostIncrementExpression : SyntaxKind.PostDecrementExpression,
            subject.Expression);

        return gen;
    }

    private ExpressionSyntax GenNewObject(FuzzType type)
    {
        if (type is not AggregateType at)
        {
            if (type is InterfaceType it)
            {
                at = _random.NextElement(_types.GetImplementingTypes(it));
            }
            else
            {
                return null;
            }
        }

        ObjectCreationExpressionSyntax creation =
            ObjectCreationExpression(at.GenReferenceTo())
            .WithArgumentList(
                ArgumentList(
                    SeparatedList(
                        at.Fields.Select(f => Argument(GenExpression(f.Type))))));

        return creation;
    }

    internal static IEnumerable<StatementSyntax> GenChecksumming(bool prefixRuntimeAccess, IEnumerable<ScopeValue> variables, Func<string> siteIdGenerator)
    {
        List<LValueInfo> paths = new();
        foreach (ScopeValue variable in variables)
            AppendVariablePaths(paths, variable);

        paths.RemoveAll(lv => !(lv.Type is PrimitiveType) && !(lv.Type is RefType rt && rt.InnerType is PrimitiveType));

        ExpressionSyntax checksumCall;

        if (prefixRuntimeAccess)
        {
            checksumCall =
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(CodeGenerator.ClassNameForStaticMethods),
                        IdentifierName("s_rt")),
                    IdentifierName("Checksum"));
        }
        else
        {
            checksumCall =
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("s_rt"),
                    IdentifierName("Checksum"));
        }

        foreach (LValueInfo lvalue in paths)
        {
            string checksumSiteId = siteIdGenerator();
            LiteralExpressionSyntax id =
                LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    Literal(checksumSiteId));

            ExpressionSyntax expr = lvalue.Expression;
            ExpressionStatementSyntax stmt =
                ExpressionStatement(
                    InvocationExpression(checksumCall)
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(new[] { Argument(id), Argument(expr) }))));

            yield return stmt.WithAdditionalAnnotations(new SyntaxAnnotation("ChecksumSiteId", checksumSiteId));
        }
    }

    private static void AppendVariablePaths(List<LValueInfo> paths, ScopeValue var)
    {
        AddPathsRecursive(var.Expression, var.Type, var.RefEscapeScope, var.ReadOnly);

        void AddPathsRecursive(ExpressionSyntax curAccess, FuzzType curType, int curRefEscapeScope, bool readOnly)
        {
            LValueInfo info = new(curAccess, curType, curRefEscapeScope, readOnly);
            paths.Add(info);

            if (curType is RefType rt)
                curType = rt.InnerType;

            switch (curType)
            {
                case ArrayType arr:
                    AddPathsRecursive(
                        ElementAccessExpression(
                            curAccess,
                            BracketedArgumentList(
                                SeparatedList(
                                    Enumerable.Repeat(
                                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0))),
                                        arr.Rank)))),
                        arr.ElementType,
                        curRefEscapeScope: int.MaxValue,
                        readOnly: false);
                    break;
                case AggregateType agg:
                    foreach (AggregateField field in agg.Fields)
                    {
                        AddPathsRecursive(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                curAccess,
                                IdentifierName(field.Name)),
                            field.Type,
                            curRefEscapeScope: agg.IsClass ? int.MaxValue : curRefEscapeScope,
                            readOnly: agg.IsClass ? false : readOnly);
                    }
                    break;
            }
        }
    }
}

internal enum StatementKind
{
    Block,
    Assignment,
    Call,
    If,
    Return,
    TryFinally,
    Loop,
}

internal enum ExpressionKind
{
    MemberAccess,
    Literal,
    Unary,
    Binary,
    Assignment,
    Call,
    Increment,
    Decrement,
    NewObject,
}
