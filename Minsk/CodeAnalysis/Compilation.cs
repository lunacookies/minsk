using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis;

public sealed class Compilation
{
    public SyntaxTree Syntax { get; }

    public Compilation(SyntaxTree syntax)
    {
        Syntax = syntax;
    }

    public EvaluationResult Evaluate()
    {
        Binder binder = new Binder();
        BoundExpression boundExpression = binder.BindExpression(Syntax.Root);

        Diagnostic[] diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();
        if (diagnostics.Any())
        {
            return new EvaluationResult(diagnostics, null);
        }

        Evaluator evaluator = new Evaluator(boundExpression);
        object value = evaluator.Evaluate();
        return new EvaluationResult(Array.Empty<Diagnostic>(), value);
    }
}