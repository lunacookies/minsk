﻿using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis;
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
            Compilation compilation = new Compilation(syntaxTree);
            EvaluationResult result = compilation.Evaluate();

            IReadOnlyList<Diagnostic> diagnostics = result.Diagnostics;

            if (showTree)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                PrettyPrint(syntaxTree.Root);
                Console.ResetColor();
            }

            if (!diagnostics.Any())
            {
                Console.WriteLine(result.Value);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (Diagnostic diagnostic in diagnostics)
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