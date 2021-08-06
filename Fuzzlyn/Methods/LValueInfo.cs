﻿using Fuzzlyn.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fuzzlyn.Methods
{
    /// <summary>
    /// Represents information about an l-value, which is an expression that can be assigned to,
    /// or have a reference taken to.
    /// </summary>
    internal class LValueInfo
    {
        public LValueInfo(ExpressionSyntax expression, FuzzType type, int refEscapeScope, bool readOnly)
        {
            Expression = expression;
            Type = type;
            RefEscapeScope = refEscapeScope;
            ReadOnly = readOnly;
        }

        public ExpressionSyntax Expression { get; }
        public FuzzType Type { get; }
        /// <summary>See <see cref="VariableIdentifier.RefEscapeScope"/>.</summary>
        public int RefEscapeScope { get; }
        public bool ReadOnly { get; }

        public override string ToString() => Expression.ToString();
    }
}
