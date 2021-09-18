// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;

    internal class HostIntegrationCodeSource: ICodeSource
    {
        internal const string UsingStatic = "using static TeamCity.CSharpInteractive.Contracts.Color;";

        public string Name => string.Empty;
        
        public bool Internal => true;

        public IEnumerator<string?> GetEnumerator()
        {
            var lines = new List<string> { UsingStatic };
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}