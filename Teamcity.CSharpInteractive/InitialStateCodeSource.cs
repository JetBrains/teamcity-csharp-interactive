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
        
        public bool Internal => true;

        public IReadOnlyCollection<string> ScriptArguments { get; set; } = Array.Empty<string>();
        
        public IReadOnlyDictionary<string, string> ScriptProperties { get; set; } = new Dictionary<string, string>();

        public IEnumerator<string> GetEnumerator()
        {
            var lines = new List<string>();
            var stringType = SyntaxFactory.ParseTypeName("System.String");
            var argsType = SyntaxFactory.ArrayType(stringType).AddRankSpecifiers(SyntaxFactory.ArrayRankSpecifier());
            var argsDeclarationStatement =
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(argsType)
                        .AddVariables(
                            SyntaxFactory.VariableDeclarator("Args")
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.ArrayCreationExpression(
                                            argsType,
                                            SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)
                                                .AddExpressions(
                                                    ScriptArguments.Select(arg => (ExpressionSyntax)CreateStringSyntax(arg)).ToArray()))))));

            lines.Add(argsDeclarationStatement.NormalizeWhitespace().ToFullString());

            var propsType = SyntaxFactory.GenericName("System.Collections.Generic.Dictionary").AddTypeArgumentListArguments(stringType, stringType);
            var propsDeclarationStatement =
                SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(propsType)
                        .AddVariables(
                            SyntaxFactory.VariableDeclarator("Props")
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.ObjectCreationExpression(propsType).AddArgumentListArguments()))));
            
            lines.Add(propsDeclarationStatement.NormalizeWhitespace().ToFullString());
            foreach (var (key, value) in ScriptProperties)
            {
                var propAssignmentStatement =
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression, 
                            SyntaxFactory.ElementAccessExpression(SyntaxFactory.IdentifierName("Props"), SyntaxFactory.BracketedArgumentList().AddArguments(SyntaxFactory.Argument(CreateStringSyntax(key)))),
                            CreateStringSyntax(value)));
                
                lines.Add(propAssignmentStatement.NormalizeWhitespace().ToFullString());
            }

            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static LiteralExpressionSyntax CreateStringSyntax(string value) => 
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(value));
    }
}