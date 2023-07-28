namespace Minsk.CodeAnalysis.Syntax;

internal sealed class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;
    public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

    public Parser(string text)
    {
        List<SyntaxToken> tokens = new List<SyntaxToken>();

        Lexer lexer = new Lexer(text);
        SyntaxToken token;

        do
        {
            token = lexer.Lex();

            if (token.Kind != SyntaxKind.WhitespaceToken &&
                token.Kind != SyntaxKind.BadToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EndOfFileToken);

        _tokens = tokens.ToArray();
        Diagnostics.AddRange(lexer.Diagnostics);
    }

    private SyntaxToken Current => Peek(0);

    private SyntaxToken Peek(int offset)
    {
        int index = _position + offset;
        if (index >= _tokens.Length)
        {
            return _tokens[^1];
        }

        return _tokens[index];
    }

    private SyntaxToken NextToken()
    {
        SyntaxToken current = Current;
        _position++;
        return current;
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
        {
            return NextToken();
        }

        Diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
        return new SyntaxToken(kind, Current.Position, null, null);
    }

    public SyntaxTree Parse()
    {
        ExpressionSyntax expression = ParseExpression();
        SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

        return new SyntaxTree(Diagnostics, expression, endOfFileToken);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        int unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
        {
            SyntaxToken operatorToken = NextToken();
            ExpressionSyntax operand = ParseExpression(unaryOperatorPrecedence);
            left = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            int precedence = Current.Kind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
            {
                break;
            }

            SyntaxToken operatorToken = NextToken();
            ExpressionSyntax right = ParseExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.OpenParenthesisToken:
            {
                SyntaxToken left = NextToken();
                ExpressionSyntax expression = ParseExpression();
                SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(
                    left,
                    expression,
                    right);
            }

            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:
            {
                SyntaxToken keywordToken = NextToken();
                bool value = keywordToken.Kind == SyntaxKind.TrueKeyword;
                return new LiteralExpressionSyntax(keywordToken, value);
            }

            default:
            {
                SyntaxToken numberToken = MatchToken(SyntaxKind.NumberToken);
                return new LiteralExpressionSyntax(numberToken);
            }
        }
    }
}