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
    EqualsEqualsToken,
    BangEqualsToken,
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