using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    public IReadOnlyList<string> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public SyntaxToken EndOfFileToken { get; }

    public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
    {
        Diagnostics = diagnostics.ToArray();
        Root = root;
        EndOfFileToken = endOfFileToken;
    }

    public static SyntaxTree Parse(string text)
    {
        Parser parser = new Parser(text);
        return parser.Parse();
    }
}