using System;
using System.Linq;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

namespace mc;

internal static class Program
{
    private static void Main()
    {
        bool showTree = false;

        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            if (line == "#showTree")
            {
                showTree = !showTree;
                Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                continue;
            }

            if (line == "#cls")
            {
                Console.Clear();
                continue;
            }

            SyntaxTree syntaxTree = SyntaxTree.Parse(line);
            Binder binder = new Binder();
            BoundExpression boundExpression = binder.BindExpression(syntaxTree.Root);

            string[] diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();

            if (showTree)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrettyPrint(syntaxTree.Root);
                Console.ResetColor();
            }

            if (!diagnostics.Any())
            {
                Evaluator e = new Evaluator(boundExpression);
                object result = e.Evaluate();
                Console.WriteLine(result);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (string diagnostic in diagnostics)
                {
                    Console.WriteLine(diagnostic);
                }

                Console.ResetColor();
            }
        }
    }

    private static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
    {
        string marker = isLast ? "└──" : "├──";

        Console.Write(indent);
        Console.Write(marker);
        Console.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            Console.Write(" ");
            Console.Write(t.Value);
        }

        Console.WriteLine();

        indent += isLast ? "   " : "│  ";

        SyntaxNode? lastChild = node.GetChildren().LastOrDefault();

        foreach (SyntaxNode child in node.GetChildren())
        {
            PrettyPrint(child, indent, child == lastChild);
        }
    }
}