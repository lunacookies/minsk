using System.Collections;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis;

internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

    public IEnumerator<Diagnostic> GetEnumerator()
    {
        return _diagnostics.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddRange(DiagnosticBag diagnostics)
    {
        _diagnostics.AddRange(diagnostics);
    }

    private void Report(TextSpan span, string message)
    {
        Diagnostic diagnostic = new Diagnostic(span, message);
        _diagnostics.Add(diagnostic);
    }

    public void ReportInvalidNumber(TextSpan span, string text, Type type)
    {
        string message = $"The number {text} isn't valid {type}.";
        Report(span, message);
    }

    public void ReportBadCharacter(int position, char character)
    {
        TextSpan span = new TextSpan(position, 1);
        string message = $"Bad character input: '{character}'.";
        Report(span, message);
    }

    public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
    {
        string message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
        Report(span, message);
    }

    public void ReportUndefinedUnaryOperator(TextSpan span, string? operatorText, Type operandType)
    {
        string message = $"Unary operator '{operatorText}' is not defined for type {operandType}.";
        Report(span, message);
    }

    public void ReportUndefinedBinaryOperator(TextSpan span, string? operatorText, Type leftType, Type rightType)
    {
        string message = $"Binary operator '{operatorText}' is not defined for types {leftType} and {rightType}.";
        Report(span, message);
    }
}