// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class InitialStateCodeSource: ICodeSource
    {
        public string Name => string.Empty;

        public IReadOnlyCollection<string> ScriptArguments { get; set; } = Array.Empty<string>();

        public IEnumerator<string> GetEnumerator()
        {
            var lines = new List<string>();
            if (ScriptArguments.Any())
            {
                var argsType = SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName(nameof(String))).AddRankSpecifiers(SyntaxFactory.ArrayRankSpecifier());
                var argsDeclarationStatement =
                    SyntaxFactory.LocalDeclarationStatement(
                        SyntaxFactory.VariableDeclaration(argsType)
                            .AddVariables(
                                SyntaxFactory.VariableDeclarator("Args")
                                    .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.ArrayCreationExpression(
                                        argsType,
                                        SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)
                                            .AddExpressions(ScriptArguments.Select(arg => (ExpressionSyntax) SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(arg))).ToArray()))))));

                lines.Add(argsDeclarationStatement.NormalizeWhitespace().ToFullString());
            }

            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}