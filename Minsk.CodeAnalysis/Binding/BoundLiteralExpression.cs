using System;

namespace Minsk.CodeAnalysis.Binding;

public sealed class BoundLiteralExpression : BoundExpression
{
    public object Value { get; }

    public BoundLiteralExpression(object value)
    {
        Value = value;
    }

    public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    public override Type Type => Value.GetType();
}