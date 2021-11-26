// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using Contracts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NuGet;

    internal class HostIntegrationCodeSource: ICodeSource
    {
        private static readonly List<string> Source = new()
        {
            SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(typeof(INuGet).Namespace ?? string.Empty))
                .NormalizeWhitespace().ToString(),
            SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName($"{typeof(Color).Namespace}.{nameof(Color)}"))
                .WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword)).NormalizeWhitespace().ToString()
        };

        public string Name => string.Empty;

        public bool Internal => true;

        public IEnumerator<string?> GetEnumerator() => Source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}