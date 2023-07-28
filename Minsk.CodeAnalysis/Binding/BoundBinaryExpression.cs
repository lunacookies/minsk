using System;

namespace Minsk.CodeAnalysis.Binding;

public sealed class BoundBinaryExpression : BoundExpression
{
    public BoundExpression Left { get; }
    public BoundBinaryOperator Op { get; }
    public BoundExpression Right { get; }

    public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override Type Type => Op.Type;
}