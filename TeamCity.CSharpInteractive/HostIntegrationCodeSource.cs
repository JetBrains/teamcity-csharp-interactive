// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    internal class HostIntegrationCodeSource: ICodeSource
    {
        private static readonly string[] ImplicitUsing = 
        {
            // .NET 6 default implicit using
            "global using global::System;",
            "global using global::System.Collections.Generic;",
            "global using global::System.IO;",
            "global using global::System.Linq;",
            "global using global::System.Net.Http;",
            "global using global::System.Threading;",
            "global using global::System.Threading.Tasks;",
            
            // Contracts
            "global using global::TeamCity.CSharpInteractive.Contracts;",
            "global using static global::TeamCity.CSharpInteractive.Contracts.Color;",
            "global using global::NuGet;"
        };

        // ReSharper disable once IdentifierTypo
        private static readonly string ImplicitUsings = string.Join(System.Environment.NewLine, ImplicitUsing);
        
        public string Name => string.Empty;

        public bool Internal => true;

        public IEnumerator<string?> GetEnumerator() => Enumerable.Repeat(ImplicitUsings, 1).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}