using System;

namespace Minsk.CodeAnalysis.Binding;

public sealed class BoundUnaryExpression : BoundExpression
{
    public BoundUnaryOperator Op { get; }
    public BoundExpression Operand { get; }

    public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
    {
        Op = op;
        Operand = operand;
    }

    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override Type Type => Operand.Type;
}