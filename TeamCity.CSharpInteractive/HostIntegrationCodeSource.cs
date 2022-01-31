// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class HostIntegrationCodeSource: ICodeSource
    {
        private static readonly IEnumerable<string> ImplicitUsing = new List<string>
        {
            System.Environment.NewLine,
            "using static Host;" + System.Environment.NewLine,
        };

        public string Name => string.Empty;

        public bool Internal => true;

        public IEnumerator<string?> GetEnumerator() => ImplicitUsing.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}