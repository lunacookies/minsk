using System;

namespace Minsk.CodeAnalysis.Binding;

public abstract class BoundExpression : BoundNode
{
    public abstract Type Type { get; }
}