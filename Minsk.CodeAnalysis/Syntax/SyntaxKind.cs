namespace Minsk.CodeAnalysis.Syntax;

public enum SyntaxKind
{
    // Tokens
    BadToken,
    EndOfFileToken,
    WhitespaceToken,
    NumberToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    BangToken,
    AmpersandAmpersandToken,
    PipePipeToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    IdentifierToken,

    // Keywords
    TrueKeyword,
    FalseKeyword,

    // Expressions
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression,
    UnaryExpression
}