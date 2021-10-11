// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using Contracts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal class HostIntegrationCodeSource: ICodeSource
    {
        public string Name => string.Empty;
        
        public bool Internal => true;

        public IEnumerator<string?> GetEnumerator() =>
            new List<string>
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName($"{typeof(Color).Namespace}.{nameof(Color)}"))
                    .WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword)).NormalizeWhitespace().ToString()
            }.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}