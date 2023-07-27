using System;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis.Binding;

public sealed class BoundUnaryOperator
{
    public SyntaxKind SyntaxKind { get; }
    public BoundUnaryOperatorKind Kind { get; }
    public Type OperandType { get; }
    public Type ResultType { get; }

    private static readonly BoundUnaryOperator[] Operators =
    {
        new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
        new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
        new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int))
    };

    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType)
        : this(syntaxKind, kind, operandType, operandType)
    {
    }

    private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType, Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        OperandType = operandType;
        ResultType = resultType;
    }

    public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, Type operandType)
    {
        foreach (BoundUnaryOperator op in Operators)
        {
            if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
            {
                return op;
            }
        }

        return null;
    }
}