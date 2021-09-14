// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    internal class HostIntegrationCodeSource: ICodeSource
    {
        private readonly IEnvironment _environment;
        internal const string UsingSystem = "using System;";
        internal const string UsingStaticColor = "using static Teamcity.CSharpInteractive.Contracts.Color;";

        public HostIntegrationCodeSource(IEnvironment environment)
        {
            _environment = environment;
        }

        public string Name => string.Empty;
        
        public bool Internal => true;

        public IEnumerator<string> GetEnumerator()
        {
            var lines = new List<string>
            {
                $"#r \"{Path.Combine(_environment.GetPath(SpecialFolder.Bin), "Teamcity.CSharpInteractive.Contracts.dll")}\"",
                UsingSystem + UsingStaticColor
            };
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}