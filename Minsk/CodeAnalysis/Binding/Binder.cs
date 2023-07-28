using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis.Binding;

internal sealed class Binder
{
    public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

    public BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.LiteralExpression:
                return BindLiteralExpression((LiteralExpressionSyntax)syntax);
            case SyntaxKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpressionSyntax)syntax);
            case SyntaxKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpressionSyntax)syntax);
            case SyntaxKind.ParenthesizedExpression:
                return BindExpression(((ParenthesizedExpressionSyntax)syntax).Expression);
            default:
                throw new Exception($"Unexpected syntax {syntax.Kind}");
        }
    }

    private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        object value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }

    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        BoundExpression boundOperand = BindExpression(syntax.Operand);
        BoundUnaryOperator? boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

        if (boundOperator == null)
        {
            Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundOperand.Type);
            return boundOperand;
        }

        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        BoundExpression boundLeft = BindExpression(syntax.Left);
        BoundExpression boundRight = BindExpression(syntax.Right);
        BoundBinaryOperator? boundOperator =
            BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

        if (boundOperator == null)
        {
            Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundLeft.Type, boundRight.Type);
            return boundLeft;
        }

        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }
}