using System;

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

            var lexer = new Lexer(line);
            while (true)
            {
                var token = lexer.NextToken();

                if (token.Kind == SyntaxKind.EndOfFileToken)
                {
                    break;
                }

                Console.Write($"{token.Kind}: '{token.Text}'");
                if (token.Value != null)
                {
                    Console.Write($" {token.Value}");
                }

                Console.WriteLine();
            }
        }
    }
}

enum SyntaxKind
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
    EndOfFileToken
}

class SyntaxToken
{
    public SyntaxKind Kind { get; }
    public int Position { get; }
    public string Text { get; }
    public object? Value { get; }

    public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }
}

class Lexer
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