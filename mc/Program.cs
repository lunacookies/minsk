using System;
using System.Collections.Generic;
using System.Linq;

namespace mc;

internal class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            var parser = new Parser(text: line);
            var expression = parser.Parse();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            PrettyPrint(expression);
            Console.ForegroundColor = color;
        }
    }

    private static void PrettyPrint(SyntaxNode node, string indent = "")
    {
        Console.Write(indent);
        Console.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            Console.Write(" ");
            Console.Write(t.Value);
        }

        Console.WriteLine();

        indent += "    ";

        foreach (var child in node.GetChildren())
        {
            PrettyPrint(child, indent);
        }
    }
}

internal enum SyntaxKind
{
    NumberToken,
    WhitespaceToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    BadToken,
    EndOfFileToken,
    NumberExpression,
    BinaryExpression
}

internal class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string? text, object? value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public override SyntaxKind Kind { get; }
    public int Position { get; }
    public string? Text { get; }
    public object? Value { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Enumerable.Empty<SyntaxNode>();
    }
}

internal class Lexer
{
    private readonly string _text;
    private int _position;

    public Lexer(string text)
    {
        _text = text;
    }

    private char Current
    {
        get
        {
            if (_position >= _text.Length)
            {
                return '\0';
            }

            return _text[_position];
        }
    }

    private void Next()
    {
        _position++;
    }

    public SyntaxToken NextToken()
    {
        if (_position >= _text.Length)
        {
            return new SyntaxToken(kind: SyntaxKind.EndOfFileToken, position: _position, text: "\0", value: null);
        }

        if (char.IsDigit(Current))
        {
            var start = _position;

            while (char.IsDigit(Current))
            {
                Next();
            }

            var length = _position - start;
            var text = _text.Substring(start, length);
            int.TryParse(text, out var value);
            return new SyntaxToken(kind: SyntaxKind.NumberToken, position: start, text: text, value: value);
        }

        if (char.IsWhiteSpace(Current))
        {
            var start = _position;

            while (char.IsWhiteSpace(Current))
            {
                Next();
            }

            var length = _position - start;
            var text = _text.Substring(start, length);
            return new SyntaxToken(kind: SyntaxKind.WhitespaceToken, position: start, text: text, value: null);
        }

        if (Current == '+')
        {
            return new SyntaxToken(kind: SyntaxKind.PlusToken, position: _position++, text: "+", value: null);
        }

        if (Current == '-')
        {
            return new SyntaxToken(kind: SyntaxKind.MinusToken, position: _position++, text: "-", value: null);
        }

        if (Current == '*')
        {
            return new SyntaxToken(kind: SyntaxKind.StarToken, position: _position++, text: "*", value: null);
        }

        if (Current == '/')
        {
            return new SyntaxToken(kind: SyntaxKind.SlashToken, position: _position++, text: "/", value: null);
        }

        if (Current == '(')
        {
            return new SyntaxToken(kind: SyntaxKind.OpenParenthesisToken, position: _position++, text: "(",
                value: null);
        }

        if (Current == ')')
        {
            return new SyntaxToken(kind: SyntaxKind.CloseParenthesisToken, position: _position++, text: ")",
                value: null);
        }

        return new SyntaxToken(
            kind: SyntaxKind.BadToken,
            position: _position++,
            text: _text.Substring(_position - 1, 1),
            value: null);
    }
}

internal abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public abstract IEnumerable<SyntaxNode> GetChildren();
}

internal abstract class ExpressionSyntax : SyntaxNode
{
}

internal sealed class NumberExpressionSyntax : ExpressionSyntax
{
    public NumberExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken;
    }

    public override SyntaxKind Kind => SyntaxKind.NumberExpression;
    public SyntaxToken NumberToken { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }
}

internal sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}

internal class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;

    public Parser(string text)
    {
        var tokens = new List<SyntaxToken>();

        var lexer = new Lexer(text: text);
        SyntaxToken token;

        do
        {
            token = lexer.NextToken();

            if (token.Kind != SyntaxKind.WhitespaceToken &&
                token.Kind != SyntaxKind.BadToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EndOfFileToken);

        _tokens = tokens.ToArray();
    }

    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Length)
        {
            return _tokens[^1];
        }

        return _tokens[index];
    }

    private SyntaxToken Current => Peek(0);

    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }

    private SyntaxToken Match(SyntaxKind kind)
    {
        if (Current.Kind == kind)
        {
            return NextToken();
        }

        return new SyntaxToken(kind: kind, position: Current.Position, text: null, value: null);
    }

    public ExpressionSyntax Parse()
    {
        var left = ParsePrimaryExpression();

        while (Current.Kind == SyntaxKind.PlusToken ||
               Current.Kind == SyntaxKind.MinusToken)
        {
            var operatorToken = NextToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left: left, operatorToken: operatorToken, right: right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        var numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken: numberToken);
    }
}