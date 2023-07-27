using System;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis.Binding;

public sealed class BoundBinaryOperator
{
    public SyntaxKind SyntaxKind { get; }
    public BoundBinaryOperatorKind Kind { get; }
    public Type LeftType { get; }
    public Type RightType { get; }
    public Type ResultType { get; }

    private static readonly BoundBinaryOperator[] Operators =
    {
        new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int)),
        new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(int)),
        new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, typeof(int)),
        new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int)),

        new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
        new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool))
    };

    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
        : this(syntaxKind, kind, type, type, type)
    {
    }

    private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType,
        Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        LeftType = leftType;
        RightType = rightType;
        ResultType = resultType;
    }

    public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
    {
        foreach (BoundBinaryOperator op in Operators)
        {
            if (op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightType == rightType)
            {
                return op;
            }
        }

        return null;
    }
}