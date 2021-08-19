﻿using Fuzzlyn.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Fuzzlyn.Reduction
{
    internal class Reducer
    {
        private readonly Rng _rng;
        private int _varCounter;
        private bool _reduceWithChildProcesses;
        private readonly string _reduceDebugGitDir;
        private readonly Stopwatch _timer = new Stopwatch();

        public Reducer(CompilationUnitSyntax original, ulong reducerSeed, bool reduceWithChildProcesses, string reduceDebugGitDir)
        {
            Original = original;
            _rng = Rng.FromSplitMix64Seed(reducerSeed);
            _reduceWithChildProcesses = reduceWithChildProcesses;
            _reduceDebugGitDir = reduceDebugGitDir;
        }

        public FuzzlynOptions Options { get; }
        public CompilationUnitSyntax Original { get; }
        public CompilationUnitSyntax Reduced { get; private set; }

        public CompilationUnitSyntax Reduce()
        {
            _timer.Restart();
            CompileResult debug = Compiler.Compile(Original, Compiler.DebugOptions);
            CompileResult release = Compiler.Compile(Original, Compiler.ReleaseOptions);

            Func<CompilationUnitSyntax, bool> isInteresting;
            if (debug.RoslynException != null || release.RoslynException != null)
            {
                CSharpCompilationOptions opts = debug.RoslynException != null ? Compiler.DebugOptions : Compiler.ReleaseOptions;
                isInteresting = program => Compiler.Compile(program, opts).RoslynException != null;
            }
            else if (debug.CompileErrors.Length > 0 || release.CompileErrors.Length > 0)
            {
                CSharpCompilationOptions opts = debug.CompileErrors.Length > 0 ? Compiler.DebugOptions : Compiler.ReleaseOptions;
                isInteresting = program =>
                {
                    CompileResult recompiled = Compiler.Compile(program, opts);
                    if (recompiled.CompileErrors.Length <= 0)
                        return false;

                    return recompiled.CompileErrors[0].Id == (debug.CompileErrors.Length > 0 ? debug.CompileErrors[0] : release.CompileErrors[0]).Id;
                };
            }
            else
            {

                var origPair = new ProgramPair(false, debug.Assembly, release.Assembly);
                RunSeparatelyResults origRunResults =
                    ProgramExecutor.RunSeparately(new List<ProgramPair> { origPair }, 20000);

                if (origRunResults.Kind == RunSeparatelyResultsKind.Timeout)
                {
                    // Currently some programs take so long that we cannot really differentiate between "hang" and "still executing".
                    // So we do not allow to reduce programs that time out.
                    throw new InvalidOperationException("Program times out");
                }

                // This variable is kind of a hack to allow the reducer to reduce programs that crash the runtime during reduction.
                // We switch to keeping this behavior if that happens.
                RunSeparatelyResultsKind interestingResult = origRunResults.Kind;

                ProgramPairResults origResults = null;
                if (origRunResults.Kind != RunSeparatelyResultsKind.Success)
                {
                    // Of course if we get a crash immediately we can just start out with it
                    _reduceWithChildProcesses = true;
                }
                else
                {
                    origResults = origRunResults.Results[0];
                    if (origResults.DebugResult.Checksum == origResults.ReleaseResult.Checksum &&
                        origResults.DebugResult.ExceptionType == origResults.ReleaseResult.ExceptionType)
                    {
                        throw new InvalidOperationException("Program has no errors");
                    }
                }

                isInteresting = prog =>
                {
                    RunSeparatelyResults results = CompileAndRun(prog, false, 20000);
                    if (results == null || results.Kind == RunSeparatelyResultsKind.Timeout)
                        return false;

                    if (interestingResult != RunSeparatelyResultsKind.Success)
                    {
                        // If we are looking for crash, then we can return a result immediately
                        return results.Kind == interestingResult;
                    }

                    if (results.Kind != RunSeparatelyResultsKind.Success)
                    {
                        interestingResult = results.Kind;
                        return true;
                    }

                    ProgramPairResults pairResult = results.Results[0];
                    // Do exceptions first because they will almost always change checksum
                    if (origResults.DebugResult.ExceptionType != origResults.ReleaseResult.ExceptionType)
                    {
                        // Must throw same exceptions in debug and release to be bad.
                        return pairResult.DebugResult.ExceptionType == origResults.DebugResult.ExceptionType &&
                               pairResult.ReleaseResult.ExceptionType == origResults.ReleaseResult.ExceptionType;
                    }
                    else
                    {
                        if (pairResult.DebugResult.ExceptionType != origResults.DebugResult.ExceptionType ||
                            pairResult.ReleaseResult.ExceptionType != origResults.ReleaseResult.ExceptionType)
                        {
                            return false;
                        }
                    }

                    return pairResult.DebugResult.Checksum != pairResult.ReleaseResult.Checksum;
                };
            }

            // Save original comments as simplification may remove it by removing an unnecessary type.
            SyntaxTriviaList originalTrivia = Original.GetLeadingTrivia();
            UpdateReduced("Removing header comments", Original.WithLeadingTrivia());
            CoarseSimplify(isInteresting);

            List<(string, IEnumerator<SyntaxNode>)> simplifierEnumerators = new List<(string, IEnumerator<SyntaxNode>)>();
            bool first = true;
            bool any = true;
            while (any)
            {
                any = false;
                while (true)
                {
                    if (!SimplifyFromList("Statements", Reduced.DescendantNodes().Where(n => n is StatementSyntax).ToList()))
                        break;
                    any = true;
                }

                while (true)
                {
                    if (!SimplifyFromList("Expressions", Reduced.DescendantNodes().Where(n => n is ExpressionSyntax).ToList()))
                        break;
                    any = true;
                }

                while (true)
                {
                    List<SyntaxNode> members =
                        Reduced.DescendantNodesAndSelf().Where(n => n is MemberDeclarationSyntax || n is CompilationUnitSyntax).ToList();

                    if (!SimplifyFromList("Members", members))
                        break;
                    any = true;
                }

                first = false;

                bool SimplifyFromList(string simplifyType, List<SyntaxNode> list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Console.Title = $"Simplifying {simplifyType}. Elapsed: {_timer.Elapsed:hh\\:mm\\:ss}. Iter: {i}/{list.Count}";

                        // Fisher-Yates shuffle.
                        int nodeIndex = _rng.Next(i, list.Count);
                        SyntaxNode node = list[nodeIndex];
                        list[nodeIndex] = list[i];

                        // Do not optimize checksum args and call itself.
                        // We still want to remove these statements, however, so we focus on the expression only.
                        InvocationExpressionSyntax invocParent = node.FirstAncestorOrSelf<InvocationExpressionSyntax>();
                        if (invocParent != null && IsChecksumCall(invocParent))
                            continue;

                        try
                        {
                            GetSimplifierEnumerators(node, !first, simplifierEnumerators);

                            while (simplifierEnumerators.Count > 0)
                            {
                                for (int j = 0; j < simplifierEnumerators.Count; j++)
                                {
                                    // Avoid blowing the variable counts up too much. Save it so we can restore it
                                    // if the new node is not interesting.
                                    int origVarCounter = _varCounter;

                                    (string name, IEnumerator<SyntaxNode> enumerator) = simplifierEnumerators[j];
                                    if (!enumerator.MoveNext())
                                    {
                                        enumerator.Dispose();
                                        simplifierEnumerators.RemoveAt(j);
                                        j--;
                                        continue;
                                    }

                                    CompilationUnitSyntax candidate = Reduced.ReplaceNode(node, enumerator.Current);
                                    if (isInteresting(candidate))
                                    {
                                        UpdateReduced($"Simplify with {name}", candidate, true);
                                        return true;
                                    }

                                    _varCounter = origVarCounter;
                                }
                            }
                        }
                        finally
                        {
                            foreach (var (_, enumerator) in simplifierEnumerators)
                                enumerator.Dispose();

                            simplifierEnumerators.Clear();
                        }
                    }

                    return false;
                }
            }

            List<SyntaxTrivia> outputComments = GetOutputComments(debug, release).Select(Comment).ToList();

            SimplifyRuntime();
            double oldSizeKiB = Original.NormalizeWhitespace().ToString().Length / 1024.0;
            double newSizeKiB = Reduced.NormalizeWhitespace().ToString().Length / 1024.0;
            string sizeComment =
                FormattableString.Invariant(
                    $"// Reduced from {oldSizeKiB:F1} KiB to {newSizeKiB:F1} KiB in {_timer.Elapsed:hh\\:mm\\:ss}");

            SyntaxTriviaList newTrivia = originalTrivia.Add(Comment(sizeComment)).AddRange(outputComments);

            UpdateReduced("Add header comments", Reduced.WithLeadingTrivia(newTrivia));

            return Reduced;
        }

        private RunSeparatelyResults CompileAndRun(CompilationUnitSyntax prog, bool trackOutput, int timeout)
        {
            CompileResult progDebug = Compiler.Compile(prog, Compiler.DebugOptions);
            CompileResult progRelease = Compiler.Compile(prog, Compiler.ReleaseOptions);

            if (progDebug.Assembly == null || progRelease.Assembly == null)
                return null;

            ProgramPair pair = new ProgramPair(trackOutput, progDebug.Assembly, progRelease.Assembly);
            RunSeparatelyResults results;
            if (_reduceWithChildProcesses)
            {
                results = ProgramExecutor.RunSeparately(new List<ProgramPair> { pair }, timeout);
            }
            else
            {
                results = new RunSeparatelyResults(RunSeparatelyResultsKind.Success, new List<ProgramPairResults> { ProgramExecutor.RunPair(pair) }, null);
            }

            return results;
        }

        private void UpdateReduced(string update, CompilationUnitSyntax newReduced, bool checkNew = false)
        {
            Trace.Assert(!checkNew || newReduced != Reduced);
            Reduced = newReduced;
            if (_reduceDebugGitDir == null)
                return;

            string path = Path.Combine(_reduceDebugGitDir, "example.cs");
            if (!Directory.Exists(_reduceDebugGitDir))
            {
                Directory.CreateDirectory(_reduceDebugGitDir);
                Git("init");
                File.WriteAllText(path, Original.NormalizeWhitespace().ToFullString());
                Git("add .");
                Git("commit -m Original");
            }

            File.WriteAllText(path, newReduced.NormalizeWhitespace().ToFullString());
            Git("add .");
            Git($"commit --allow-empty -m \"{update}\"");

            void Git(string cmd)
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "git",
                        Arguments = cmd,
                        WorkingDirectory = _reduceDebugGitDir,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
            }
        }


        /// <summary>
        /// Perform coarse simplification by removing statements with a binary search.
        /// </summary>
        private void CoarseSimplify(Func<CompilationUnitSyntax, bool> isInteresting)
        {
            // Step 1: Declare all non-ref variables at the start of the methods with a default value.
            // This will hopefully allow us to remove more assignments than otherwise.
            Dictionary<SyntaxNode, SyntaxNode> replacements = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (MethodDeclarationSyntax method in Reduced.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                List<LocalDeclarationStatementSyntax> decls =
                    method.DescendantNodes()
                    .OfType<LocalDeclarationStatementSyntax>()
                    .Where(decl => decl.Declaration.Variables.Count == 1 &&
                                   decl.Declaration.Variables[0] != null &&
                                   !(decl.Declaration.Type is RefTypeSyntax))
                    .ToList();

                if (decls.Count == 0)
                    continue;

                Dictionary<SyntaxNode, SyntaxNode> assignmentReplacements = new Dictionary<SyntaxNode, SyntaxNode>();
                foreach (LocalDeclarationStatementSyntax decl in decls)
                {
                    assignmentReplacements.Add(
                        decl,
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(decl.Declaration.Variables[0].Identifier),
                                    decl.Declaration.Variables[0].Initializer.Value)));
                }

                MethodDeclarationSyntax withoutLocals =
                    method.ReplaceNodes(
                        assignmentReplacements.Keys,
                        (orig, _) => assignmentReplacements[orig]);

                IEnumerable<LocalDeclarationStatementSyntax> newDecls =
                    decls
                    .Select(
                        d => d.ReplaceNode(
                            d.Declaration.Variables[0].Initializer.Value,
                            DefaultExpression(d.Declaration.Type)));

                MethodDeclarationSyntax withLocalsAtTop =
                    withoutLocals.WithBody(
                        Block(
                            withoutLocals.Body.Statements.InsertRange(
                                0,
                                newDecls)));

                replacements.Add(method, withLocalsAtTop);
            }

            CompilationUnitSyntax localsMoved = Reduced.ReplaceNodes(replacements.Keys, (orig, _) => replacements[orig]);
            if (isInteresting(localsMoved))
                UpdateReduced("Move locals up", localsMoved);

            // Step 2: Remove by binary searches. Prefer large ones first to remove as much as fast as possible.
            List<string> names =
                Reduced.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .OrderByDescending(m => m.DescendantNodes().Count())
                .Select(m => m.Identifier.Text)
                .ToList();

            for (int i = 0; i < names.Count; i++)
            {
                Console.Title = $"Simplifying coarsely. Elapsed: {_timer.Elapsed:hh\\:mm\\:ss}. Method {i + 1}/{names.Count}.";
                bool isMethodInteresting(MethodDeclarationSyntax orig, MethodDeclarationSyntax @new)
                    => isInteresting(Reduced.ReplaceNode(orig, @new));

                var method = Reduced.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == names[i]);
                var newMethod = (MethodDeclarationSyntax)new CoarseStatementRemover(isMethodInteresting).Visit(method);
                UpdateReduced($"Bulk remove statements in {names[i]}", Reduced.ReplaceNode(method, newMethod));
            }
        }

        private IEnumerable<string> GetOutputComments(CompileResult ogDebugCompile, CompileResult ogRelCompile)
        {
            if (ogDebugCompile.RoslynException != null)
            {
                yield return $"// Roslyn throws '{ogDebugCompile.RoslynException.GetType()}' when compiling in debug";
                yield break;
            }
            if (ogRelCompile.RoslynException != null)
            {
                yield return $"// Roslyn throws '{ogRelCompile.RoslynException.GetType()}' when compiling in release";
                yield break;
            }

            if (ogDebugCompile.CompileErrors.Length > 0 || ogRelCompile.CompileErrors.Length > 0)
            {
                CSharpCompilationOptions compileOpts =
                    ogDebugCompile.CompileErrors.Length > 0 ? Compiler.DebugOptions : Compiler.ReleaseOptions;

                CompileResult result = Compiler.Compile(Reduced.NormalizeWhitespace(), compileOpts);
                yield return $"// Roslyn gives '{result.CompileErrors[0]}'";
                yield break;
            }

            RunSeparatelyResults results = CompileAndRun(Reduced, true, 20000);
            if (results == null)
            {
                yield return $"// Unexpected compiler error";
                yield break;
            }

            switch (results.Kind)
            {
                case RunSeparatelyResultsKind.Crash:
                    if (!string.IsNullOrWhiteSpace(results.CrashError))
                    {
                        yield return $"// Exits with error:";
                        foreach (string line in results.CrashError.Replace("\r", "").Split('\n'))
                            yield return $"// {line}";

                        yield break;
                    }

                    yield return $"// Crashes the runtime";
                    yield break;
                case RunSeparatelyResultsKind.Timeout:
                    yield return $"// Times out";
                    yield break;
                case RunSeparatelyResultsKind.Success:
                    var pairResult = results.Results[0];
                    yield return $"// Debug: {FormatResult(pairResult.DebugResult, pairResult.DebugFirstUnmatch)}";
                    yield return $"// Release: {FormatResult(pairResult.ReleaseResult, pairResult.ReleaseFirstUnmatch)}";
                    break;

                    string FormatResult(ProgramResult result, ChecksumSite unmatch)
                    {
                        if (pairResult.DebugResult?.ExceptionType != null ||
                            pairResult.ReleaseResult?.ExceptionType != null)
                        {
                            if (result.ExceptionType != null)
                                return $"Throws '{result.ExceptionType}'";

                            return "Runs successfully";
                        }

                        if (pairResult.DebugResult?.ChecksumSites?.Count != pairResult.ReleaseResult?.ChecksumSites?.Count)
                            return $"Prints {result.ChecksumSites.Count} line(s)";

                        if (unmatch != null)
                            return $"Outputs {unmatch.Value}";

                        return "";
                    }
            }
        }

        /// <summary>
        /// Simplifies everything related to the runtime; removes the s_rt field,
        /// and associated assignments, and converts checksum calls into Console.WriteLine.
        /// </summary>
        private void SimplifyRuntime()
        {
            // First remove argument to Main
            MethodDeclarationSyntax mainMethod =
                Reduced.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "Main");

            CompilationUnitSyntax withArgRemoved =
                Reduced.ReplaceNode(mainMethod, mainMethod.WithParameterList(ParameterList()));

            Dictionary<SyntaxNode, SyntaxNode> replacements = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (SyntaxNode node in withArgRemoved.DescendantNodes())
            {
                // Remove s_rt field
                if (node is FieldDeclarationSyntax field && field.Declaration.Variables.Count == 1 &&
                    field.Declaration.Variables[0].Identifier.Text == "s_rt")
                {
                    replacements.Add(node, null);
                    continue;
                }

                if (node is not ExpressionStatementSyntax expStmt)
                    continue;

                // Remove s_rt = rt
                if (expStmt.Expression is AssignmentExpressionSyntax asgn && asgn.Left is IdentifierNameSyntax id &&
                    id.Identifier.Text == "s_rt")
                {
                    replacements.Add(node, null);
                    continue;
                }

                // Convert s_rt.Checksum() calls to System.Console.WriteLine
                if (expStmt.Expression is InvocationExpressionSyntax invoc && IsChecksumCall(invoc))
                {
                    ArgumentSyntax arg = invoc.ArgumentList.Arguments[1];
                    ExpressionStatementSyntax newCall =
                        ExpressionStatement(
                            InvocationExpression(
                                ParseExpression("System.Console.WriteLine"),
                                ArgumentList(
                                    SingletonSeparatedList(arg))));

                    replacements.Add(node, newCall);
                }
            }

            UpdateReduced("Remove runtime code", withArgRemoved.ReplaceNodes(replacements.Keys, (orig, _) => replacements[orig]));
        }

        private bool IsChecksumCall(InvocationExpressionSyntax invoc)
        {
            return invoc.Expression is MemberAccessExpressionSyntax mem &&
                   mem.Name.Identifier.Text == "Checksum";
        }

        private readonly List<(string name, SimplifierAttribute info, Func<SyntaxNode, object> simp)> _simplifiers =
            new List<(string name, SimplifierAttribute info, Func<SyntaxNode, object> simp)>();

        private void GetSimplifierEnumerators(
            SyntaxNode node, bool late, List<(string name, IEnumerator<SyntaxNode> enumerator)> simplifyEnumerators)
        {
            if (_simplifiers.Count == 0)
            {
                MethodInfo[] methods = typeof(Reducer).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                IEnumerable<MethodInfo> prioritized =
                    methods
                    .Select(m => (method: m, attrib: m.GetCustomAttribute<SimplifierAttribute>()))
                    .Where(t => t.attrib != null)
                    .OrderByDescending(t => t.attrib.Priority)
                    .Select(t => t.method);

                foreach (MethodInfo method in prioritized)
                {
                    SimplifierAttribute attrib = method.GetCustomAttribute<SimplifierAttribute>();
                    if (attrib == null)
                        continue;

                    Delegate dlg;
                    if (method.ReturnType == typeof(SyntaxNode))
                        dlg = Delegate.CreateDelegate(typeof(Func<SyntaxNode, SyntaxNode>), this, method);
                    else
                        dlg = Delegate.CreateDelegate(typeof(Func<SyntaxNode, IEnumerable<SyntaxNode>>), this, method);

                    _simplifiers.Add((method.Name, attrib, (Func<SyntaxNode, object>)dlg));
                }
            }

            foreach (var (name, info, simplifier) in _simplifiers)
            {
                if (info.Late && !late)
                    continue;

                object simplified = simplifier(node);
                IEnumerator<SyntaxNode> enumerator = null;
                if (simplified == null || simplified is SyntaxNode)
                {
                    if (simplified != node)
                        enumerator = new List<SyntaxNode>(1) { (SyntaxNode)simplified }.GetEnumerator();
                }
                else
                    enumerator = ((IEnumerable<SyntaxNode>)simplified).GetEnumerator();

                if (enumerator != null)
                    simplifyEnumerators.Add((name, enumerator));
            }
        }

        // Simplify statement containing call to just the call.
        [Simplifier]
        private IEnumerable<SyntaxNode> SimplifyStatementWithCall(SyntaxNode node)
        {
            if (node is not ExpressionStatementSyntax stmt)
                yield break;

            // Take descendant nodes of expression to avoid simplifying M(); to M();
            List<InvocationExpressionSyntax> calls = stmt.Expression.DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();
            if (calls.Count <= 0)
                yield break;

            foreach (InvocationExpressionSyntax invoc in calls)
                yield return ExpressionStatement(invoc);
        }

        // Simplify "int a = expr;" to "int a;"
        [Simplifier]
        private SyntaxNode SimplifyLocalRemoveInitializer(SyntaxNode node)
        {
            if (node is not LocalDeclarationStatementSyntax local || local.Declaration.Variables.Count != 1)
                return node;

            return
                local.WithDeclaration(
                    local.Declaration
                    .WithVariables(
                        SingletonSeparatedList(
                            local.Declaration.Variables[0].WithInitializer(null))));
        }

        [Simplifier]
        private IEnumerable<SyntaxNode> MakeInstanceMethodsStatic(SyntaxNode node)
        {
            if (node is not CompilationUnitSyntax comp)
                yield break;

            ClassDeclarationSyntax programClass =
                comp.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(cls => cls.Identifier.ValueText == CodeGenerator.ClassNameForStaticMethods);
            if (programClass == null)
                yield break;

            foreach (TypeDeclarationSyntax type in comp.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (type == programClass)
                    continue;

                foreach (MemberDeclarationSyntax member in type.Members)
                {
                    if (member is not MethodDeclarationSyntax meth)
                        continue;

                    if (meth.Modifiers.Any(mod => mod.Kind() == SyntaxKind.StaticKeyword))
                        continue;

                    // Add this parameter. Currently this is never byref although we might want to do that for structs.
                    ParameterSyntax thisParam =
                        Parameter(
                            Identifier("argThis"))
                        .WithType(IdentifierName(type.Identifier));

                    ExpressionSyntax useThisArg = IdentifierName("argThis");
                    MethodDeclarationSyntax newMeth = meth.WithParameterList(ParameterList(meth.ParameterList.Parameters.Insert(0, thisParam)));
                    // Replace uses in body of 'this' with arg
                    newMeth =
                        newMeth.ReplaceNodes(
                            newMeth.DescendantNodes().OfType<ThisExpressionSyntax>(),
                            (origNode, node) => useThisArg);

                    SyntaxNode CreateNewMemberAccess(SyntaxNode origNode, SyntaxNode node)
                    {
                        if (node is not MemberAccessExpressionSyntax mem)
                            return node;

                        if (mem.Expression is not IdentifierNameSyntax id)
                            return node;

                        if (id.Identifier.ValueText != programClass.Identifier.ValueText)
                            return node;

                        return mem.Name;
                    }

                    // Replace references to 'Program' in body
                    newMeth =
                        newMeth.ReplaceNodes(
                            newMeth.DescendantNodes(),
                            CreateNewMemberAccess);

                    // Add static
                    newMeth = newMeth.AddModifiers(Token(SyntaxKind.StaticKeyword));

                    SyntaxNode CreateNewInvoc(SyntaxNode origNode, SyntaxNode node)
                    {
                        if (node is not InvocationExpressionSyntax invoc)
                            return node;

                        if (invoc.Expression is not MemberAccessExpressionSyntax mem)
                            return node;

                        if (mem.Name is not IdentifierNameSyntax id)
                            return node;

                        if (id.Identifier.ValueText != meth.Identifier.ValueText)
                            return node;

                        ExpressionSyntax access;
                        if (node.FirstAncestorOrSelf<ClassDeclarationSyntax>()?.Identifier.ValueText == programClass.Identifier.ValueText)
                            access = IdentifierName(meth.Identifier.ValueText);
                        else
                            access = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(programClass.Identifier.ValueText), IdentifierName(meth.Identifier.ValueText));

                        return invoc.WithExpression(access).WithArgumentList(ArgumentList(invoc.ArgumentList.Arguments.Insert(0, Argument(mem.Expression))));
                    }

                    var reps = new Dictionary<SyntaxNode, SyntaxNode>
                    {
                        [programClass] = programClass.AddMembers(newMeth),
                        [meth] = null,
                    };
                    // Add new member into class and remove old member
                    CompilationUnitSyntax compWithMovedFunc =
                        comp.ReplaceNodes(reps.Keys, (origNode, node) => reps[origNode]);

                    // Now replace calls to this function globally
                    compWithMovedFunc = compWithMovedFunc.ReplaceNodes(compWithMovedFunc.DescendantNodes(), CreateNewInvoc);

                    yield return compWithMovedFunc;
                }
            }
        }

        // Inline calls. We only do this late since it is better to reduce each call before inlining.
        [Simplifier(Late = true)]
        private IEnumerable<SyntaxNode> InlineCall(SyntaxNode node)
        {
            if (node is not ClassDeclarationSyntax cls)
                yield break;

            IEnumerable<InvocationExpressionSyntax> invocs =
                cls.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression is IdentifierNameSyntax);

            foreach (InvocationExpressionSyntax invoc in invocs)
            {
                MethodDeclarationSyntax target =
                    cls.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .FirstOrDefault(m => m.Identifier.Text == ((IdentifierNameSyntax)invoc.Expression).Identifier.Text);

                // Cannot yet inline functions that have multiple returns, or returns in different position than last...
                int numReturns = target.DescendantNodes().Count(s => s is ReturnStatementSyntax);
                if (numReturns > 1 || (numReturns == 1 && !(target.Body.Statements.Last() is ReturnStatementSyntax)))
                    continue;

                Debug.Assert(target != null);

                List<StatementSyntax> finalStatements = new List<StatementSyntax>();
                Dictionary<string, string> idReplacements = new Dictionary<string, string>();
                // Since we may assign to arguments, we need to introduce a local for each argument.
                // We also need to do this because a variable may go through an implicit conversion when
                // being passed as an arg, so the assignments here replicate that as well.
                foreach (var (param, arg) in target.ParameterList.Parameters.Zip(invoc.ArgumentList.Arguments, (p, a) => (p, a)))
                {
                    LocalDeclarationStatementSyntax argLocal;
                    string argLocalName;
                    if (!arg.RefKindKeyword.IsKind(SyntaxKind.None))
                        (argLocal, argLocalName) = MakeLocalDecl(RefExpression(arg.Expression), RefType(param.Type));
                    else
                        (argLocal, argLocalName) = MakeLocalDecl(arg.Expression, param.Type);

                    finalStatements.Add(argLocal);
                    idReplacements.Add(param.Identifier.Text, argLocalName);
                }

                // Rename locals to avoid clashes
                foreach (VariableDeclaratorSyntax varDecl in
                    target.Body.DescendantNodes().OfType<VariableDeclaratorSyntax>())
                {
                    idReplacements.Add(varDecl.Identifier.Text, MakeLocalName());
                }

                string valueName = null;
                foreach (StatementSyntax stmt in target.Body.Statements)
                {
                    IEnumerable<SyntaxToken> tokens =
                        stmt.DescendantTokens()
                            .Where(t => t.IsKind(SyntaxKind.IdentifierToken) && idReplacements.ContainsKey(t.Text));

                    StatementSyntax newStmt =
                        stmt.ReplaceTokens(tokens, (orig, _) => Identifier(idReplacements[orig.Text]));

                    if (newStmt is ReturnStatementSyntax ret)
                    {
                        if (ret.Expression == null)
                            continue;

                        (newStmt, valueName) = MakeLocalDecl(ret.Expression, target.ReturnType);
                    }

                    finalStatements.Add(newStmt);
                }

                bool replaceUsage = !(invoc.Parent is ExpressionStatementSyntax);
                Debug.Assert(!replaceUsage || valueName != null, "We need to replace usage but no return statement was found");

                StatementSyntax containingInvoc = invoc.FirstAncestorOrSelf<StatementSyntax>();
                if (replaceUsage)
                {
                    finalStatements.Add(containingInvoc.ReplaceNode(invoc, IdentifierName(valueName)));
                }

                BlockSyntax containingBlock = invoc.FirstAncestorOrSelf<BlockSyntax>();
                SyntaxNode newNode =
                    node.ReplaceNode(
                        containingBlock,
                        containingBlock.WithStatements(
                            containingBlock.Statements.ReplaceRange(containingInvoc, finalStatements)));

                yield return newNode;
            }
        }

        // Remove statements
        [Simplifier(Priority = 1)]
        private SyntaxNode SimplifyStatementRemove(SyntaxNode node)
        {
            if (!(node is StatementSyntax) || !(node.Parent is BlockSyntax))
                return node;

            return null;
        }

        [Simplifier]
        private IEnumerable<SyntaxNode> SimplifyIf(SyntaxNode node)
        {
            if (node is not IfStatementSyntax @if)
                yield break;

            // Simplify to then block
            yield return @if.Statement;

            BlockSyntax thenBlock = @if.Statement as BlockSyntax;

            // Extract condition for empty if
            if (thenBlock != null && thenBlock.Statements.Count == 0 && @if.Else == null)
            {
                var (local, name) = MakeLocalDecl(@if.Condition, PredefinedType(Token(SyntaxKind.BoolKeyword)));
                IfStatementSyntax newIf = @if.WithCondition(IdentifierName(name));
                yield return Block(local, newIf);
            }

            if (@if.Else == null)
                yield break;

            // Simplify to else block
            yield return @if.Else.Statement;
            // Remove else block
            yield return @if.WithElse(null);

            // Flip if (expr) { } else { else; }  to "if (!expr) { else; }"
            if (thenBlock != null && thenBlock.Statements.Count <= 0)
            {
                IfStatementSyntax newIf =
                    IfStatement(
                        PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            ParenthesizedExpression(@if.Condition)),
                    @if.Else.Statement);

                yield return newIf;
            }
        }

        [Simplifier]
        private IEnumerable<SyntaxNode> SimplifyFor(SyntaxNode node)
        {
            if (node is not ForStatementSyntax @for)
                yield break;

            // Simplify 'for (var a = expr; b; c) stmt' to
            // var a = expr; stmt
            VariableDeclarationSyntax decl = @for.Declaration;
            StatementSyntax body = @for.Statement;
            yield return Block(LocalDeclarationStatement(decl), body);
        }

        [Simplifier]
        private IEnumerable<SyntaxNode> SimplifyBinaryExpression(SyntaxNode node)
        {
            if (node is not BinaryExpressionSyntax bin)
                yield break;

            yield return bin.Left;
            yield return bin.Right;
        }

        [Simplifier]
        private SyntaxNode SimplifyCast(SyntaxNode node)
        {
            if (node is not CastExpressionSyntax cast)
                return node;

            return cast.Expression;
        }

        [Simplifier]
        private SyntaxNode SimplifyParenthesized(SyntaxNode node)
        {
            if (node is not ParenthesizedExpressionSyntax p)
                return node;

            if (Helpers.RequiresParentheses(p.Expression) &&
                !(node.Parent is EqualsValueClauseSyntax))
            {
                return node;
            }

            return p.Expression;
        }

        [Simplifier]
        private SyntaxNode SimplifyPrefixUnaryExpression(SyntaxNode node)
        {
            if (node is not PrefixUnaryExpressionSyntax p)
                return node;

            return p.Operand;
        }

        [Simplifier]
        private SyntaxNode SimplifyPostfixUnaryExpression(SyntaxNode node)
        {
            if (node is not PostfixUnaryExpressionSyntax p)
                return node;

            return p.Operand;
        }

        [Simplifier]
        private SyntaxNode SimplifyInitializer(SyntaxNode node)
        {
            if (node is not InitializerExpressionSyntax init)
                return node;

            if (init.Kind() != SyntaxKind.ArrayInitializerExpression || init.Expressions.Count < 1)
                return node;

            return init.WithExpressions(SingletonSeparatedList(init.Expressions[0]));
        }

        [Simplifier]
        private SyntaxNode SimplifyBlockFlatten(SyntaxNode node)
        {
            if (node is not BlockSyntax block || !block.Statements.Any(s => s is BlockSyntax))
                return node;

            return block.WithStatements(
                block.Statements.SelectMany(s => s is BlockSyntax bs ? bs.Statements : Enumerable.Repeat(s, 1)).ToSyntaxList());
        }

        [Simplifier]
        private SyntaxNode RemoveMethodDeclaration(SyntaxNode node)
        {
            if (node is not MethodDeclarationSyntax method || method.Identifier.Text == "Main")
                return node;

            return null;
        }

        [Simplifier]
        private SyntaxNode RemoveTypeDeclaration(SyntaxNode node)
        {
            if (node is not TypeDeclarationSyntax type || type.Identifier.Text == "Program")
                return node;

            return null;
        }

        [Simplifier]
        private IEnumerable<SyntaxNode> SimplifyField(SyntaxNode node)
        {
            if (node is not FieldDeclarationSyntax field)
                yield break;

            // Remove
            yield return null;

            if (field.Declaration.Variables.Count == 1 &&
                field.Declaration.Variables[0].Initializer != null)
            {
                // Remove initializer
                yield return field.ReplaceNode(field.Declaration.Variables[0].Initializer, (SyntaxNode)null);
            }

        }

        [Simplifier]
        private IEnumerable<SyntaxNode> RemoveMethodArgument(SyntaxNode node)
        {
            if (node is not CompilationUnitSyntax unit)
                yield break;

            IEnumerable<(ParameterSyntax pm, int index)> methodParams =
                unit.DescendantNodes().OfType<BaseMethodDeclarationSyntax>()
                .Where(m => m is not MethodDeclarationSyntax normalMethod || normalMethod.Identifier.Text != "Main")
                .SelectMany(m => m.ParameterList.Parameters.Select((p, i) => (p, i)));

            foreach (var (parameter, index) in methodParams)
            {
                BaseMethodDeclarationSyntax method = (BaseMethodDeclarationSyntax)parameter.Parent.Parent;
                Func<SyntaxNode, bool> isInvoc;
                Func<SyntaxNode, ArgumentListSyntax> getInvocArgs;
                if (method is MethodDeclarationSyntax)
                {
                    isInvoc = n => n is InvocationExpressionSyntax invoc &&
                              invoc.Expression is IdentifierNameSyntax id &&
                              id.Identifier.Text == ((MethodDeclarationSyntax)method).Identifier.Text;
                    getInvocArgs = n => ((InvocationExpressionSyntax)n).ArgumentList;
                }
                else if (method is ConstructorDeclarationSyntax)
                {
                    isInvoc = n => n is ObjectCreationExpressionSyntax creation &&
                                   creation.Type is IdentifierNameSyntax id &&
                                   id.Identifier.Text == ((ConstructorDeclarationSyntax)method).Identifier.Text;
                    getInvocArgs = n => ((ObjectCreationExpressionSyntax)n).ArgumentList;
                }
                else
                    continue;

                SyntaxNode methodWithoutParam;
                if (method is ConstructorDeclarationSyntax && method.ParameterList.Parameters.Count == 1)
                {
                    // Remove constructors if they get 0 args
                    methodWithoutParam = null;
                }
                else
                {
                    methodWithoutParam =
                        method.ReplaceNode(
                            method.ParameterList,
                            method.ParameterList.WithParameters(method.ParameterList.Parameters.RemoveAt(index)));
                }

                SyntaxNode newNode = unit.ReplaceNode(method, methodWithoutParam);
                while (true)
                {
                    SyntaxNode invoc =
                        newNode.DescendantNodes().FirstOrDefault(
                            i => isInvoc(i) && getInvocArgs(i).Arguments.Count == method.ParameterList.Parameters.Count);

                    if (invoc == null)
                        break;

                    ArgumentListSyntax args = getInvocArgs(invoc);
                    ArgumentListSyntax newArgs = args.WithArguments(args.Arguments.RemoveAt(index));
                    newNode = newNode.ReplaceNode(args, newArgs);
                }

                yield return newNode;
            }
        }

        // Simplify an invocation by extracting only a arg. In some cases
        // we get rid of the interesting behavior by extracting all args, so we can
        // fall back to this in that case (which is also why this is marked late).
        [Simplifier(Late = true)]
        private IEnumerable<SyntaxNode> SimplifyInvocationExtractArg(SyntaxNode node)
        {
            // Simplify a random invocation in a block from
            // M(expr1, expr2)
            // or
            // a = M(expr1, expr2)
            // or
            // var a = M(expr1, expr2)
            // to
            // var arg1 = expr1;
            // [var a = ] M(arg1, expr2)
            // for each argument

            if (node is not BlockSyntax block)
                yield break;

            IEnumerable<InvocationExpressionSyntax> invocs =
                block.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => i.FirstAncestorOrSelf<BlockSyntax>() == block);

            foreach (InvocationExpressionSyntax invoc in invocs)
            {
                if (IsChecksumCall(invoc))
                    continue;

                int statementIndex =
                    block.Statements.Select((s, i) => (s, i)).Single(t => t.s.Contains(invoc)).i;

                // If we enumerate in order we will always extract args so that the evaluation order is still correct.
                foreach (ArgumentSyntax arg in invoc.ArgumentList.Arguments)
                {
                    if (arg.Expression is IdentifierNameSyntax || arg.Expression is LiteralExpressionSyntax)
                        continue;

                    var (local, name) = MakeLocalDecl(arg.Expression);
                    BlockSyntax newBlock = block.ReplaceNode(arg, Argument(IdentifierName(name)));
                    newBlock = newBlock.WithStatements(newBlock.Statements.Insert(statementIndex, local));
                    yield return newBlock;
                }
            }
        }

        // Combine code like "ulong var0; var0 = 123" to "ulong var0 = 123"
        [Simplifier]
        private IEnumerable<SyntaxNode> CombineLocalAssignmentsInBlock(SyntaxNode node)
        {
            if (node is not BlockSyntax block)
                yield break;

            foreach (StatementSyntax statement in block.Statements)
            {
                if (statement is not LocalDeclarationStatementSyntax local)
                    continue;

                if (local.Declaration.Variables.Count != 1 ||
                    local.Declaration.Variables[0].Initializer != null)
                    continue;

                string localId = local.Declaration.Variables[0].Identifier.Text;
                foreach (ExpressionStatementSyntax expStmt in block.DescendantNodes().OfType<ExpressionStatementSyntax>())
                {
                    if (expStmt.Expression is not AssignmentExpressionSyntax asgn)
                        continue;

                    if (asgn.Left is not IdentifierNameSyntax id)
                        continue;

                    if (id.Identifier.Text != localId)
                        continue;

                    LocalDeclarationStatementSyntax newLocal =
                        local
                        .WithDeclaration(
                            local.Declaration.WithVariables(
                                SingletonSeparatedList(
                                    local.Declaration.Variables[0].WithInitializer(
                                        EqualsValueClause(asgn.Right)))));

                    int localIndex = block.Statements.IndexOf(local);

                    BlockSyntax newBlock =
                        block.ReplaceNode(expStmt, newLocal);

                    newBlock = newBlock.WithStatements(newBlock.Statements.RemoveAt(localIndex));
                    yield return newBlock;
                    // Only check with first assignment
                    break;
                }
            }
        }

        // Inlines locals of the forms
        // var a = b;
        // var a = literal;
        // ref int a = ref b;
        // All occurences of a are replaced by the right-hand side.
        [Simplifier]
        private IEnumerable<SyntaxNode> InlineLocals(SyntaxNode node)
        {
            if (node is not BlockSyntax block)
                yield break;

            foreach (LocalDeclarationStatementSyntax local in block.Statements.OfType<LocalDeclarationStatementSyntax>())
            {
                if (local.Declaration.Variables.Count != 1)
                    continue;

                VariableDeclaratorSyntax var = local.Declaration.Variables[0];
                if (var.Initializer == null)
                    continue;

                bool isIdentifier = var.Initializer.Value is IdentifierNameSyntax;
                bool isLiteral = var.Initializer.Value is LiteralExpressionSyntax;
                bool isRefIdentifier = var.Initializer.Value is RefExpressionSyntax lclRes &&
                                       lclRes.Expression is IdentifierNameSyntax;

                if (!isIdentifier && !isLiteral && !isRefIdentifier)
                    continue;

                string toReplace = local.Declaration.Variables[0].Identifier.Text;
                SyntaxNode replaceWith = local.Declaration.Variables[0].Initializer.Value;

                if (replaceWith is RefExpressionSyntax res)
                    replaceWith = res.Expression;

                BlockSyntax newNode =
                    block.WithStatements(block.Statements.Remove(local));

                newNode =
                    newNode.ReplaceNodes(
                        newNode.DescendantNodes().OfType<IdentifierNameSyntax>().Where(id => id.Identifier.Text == toReplace),
                        (orig, _) => replaceWith);

                yield return newNode;
            }
        }

        [Simplifier(Late = true)]
        private IEnumerable<SyntaxNode> SimplifyConstant(SyntaxNode node)
        {
            if (node is not LiteralExpressionSyntax literal || literal.Kind() != SyntaxKind.NumericLiteralExpression)
                yield break;

            if (literal.Token.Text == "0" || literal.Token.Text == "1" || literal.Token.Text == "-1")
                yield break;

            yield return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0));
            yield return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(1));
            yield return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(-1));
        }

        // Add ': this()' to struct constructor to allow further simplification of struct
        [Simplifier]
        private SyntaxNode SimplifyStructAddConstructorInitializers(SyntaxNode node)
        {
            if (node is not StructDeclarationSyntax @struct)
                return node;

            StructDeclarationSyntax newStruct = @struct;
            while (true)
            {
                ConstructorDeclarationSyntax ctor =
                    newStruct.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault(c => c.Initializer == null);

                if (ctor == null)
                    return newStruct;

                newStruct = newStruct.ReplaceNode(
                    ctor,
                    ctor.WithInitializer(
                        ConstructorInitializer(
                            SyntaxKind.ThisConstructorInitializer,
                            ArgumentList())));
            }
        }

        // Simplify ref-local. ref T a = ref b => T a = b
        [Simplifier]
        private SyntaxNode SimplifyLocalRemoveRef(SyntaxNode node)
        {
            if (node is not LocalDeclarationStatementSyntax local || local.Declaration.Variables.Count != 1 ||
                local.Declaration.Type is not RefTypeSyntax refTypeSyntax)
                return node;

            VariableDeclaratorSyntax var = local.Declaration.Variables[0];
            if (var.Initializer == null)
                return node;

            Debug.Assert(var.Initializer.Value is RefExpressionSyntax);

            ExpressionSyntax innerExpr = ((RefExpressionSyntax)var.Initializer.Value).Expression;
            SyntaxNode newNode =
                LocalDeclarationStatement(
                    VariableDeclaration(
                        refTypeSyntax.Type,
                        SingletonSeparatedList(
                            VariableDeclarator(
                                var.Identifier)
                            .WithInitializer(
                                EqualsValueClause(
                                    innerExpr)))));

            return newNode;
        }

        [Simplifier]
        private IEnumerable<SyntaxNode> SimplifyTryFinally(SyntaxNode node)
        {
            if (node is not TryStatementSyntax @try || @try.Catches.Any())
                yield break;

            yield return @try.Block;
            yield return @try.Finally.Block;
            yield return Block(@try.Block, @try.Finally.Block);
            // Try finally first and then block. Useful when the try-block contains a
            // return.
            yield return Block(@try.Finally.Block, @try.Block);
        }

        [Simplifier]
        private SyntaxNode SimplifyMethodRemoveReturn(SyntaxNode node)
        {
            if (node is not MethodDeclarationSyntax method)
                return node;

            if (method.ReturnType is PredefinedTypeSyntax returnType &&
                returnType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return node;
            }

            SyntaxNode newNode =
                method.WithReturnType(PredefinedType(Token(SyntaxKind.VoidKeyword)));
            newNode = newNode.ReplaceNodes(newNode.DescendantNodes().OfType<ReturnStatementSyntax>(), (cur, old) => ReturnStatement());
            return newNode;
        }

        private (LocalDeclarationStatementSyntax local, string name) MakeLocalDecl(ExpressionSyntax expr, TypeSyntax type = null)
        {
            string name = MakeLocalName();

            LocalDeclarationStatementSyntax local =
                LocalDeclarationStatement(
                    VariableDeclaration(
                        type ?? IdentifierName("var"),
                        SingletonSeparatedList(
                            VariableDeclarator(name)
                            .WithInitializer(
                                EqualsValueClause(
                                    expr)))));

            return (local, name);
        }

        private string MakeLocalName() => $"vr{_varCounter++}";

        [AttributeUsage(AttributeTargets.Method)]
        private class SimplifierAttribute : Attribute
        {
            public int Priority { get; set; }
            public bool Late { get; set; }
        }
    }
}
