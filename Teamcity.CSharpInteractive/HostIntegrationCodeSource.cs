// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;

    internal class HostIntegrationCodeSource: ICodeSource
    {
        private const string LoadHost = "#r \"Teamcity.Host.dll\"";
        private const string UsingHost = "using Teamcity.Host;";
        private const string UsingStaticHost = "using static Teamcity.Host.Host;";
        private const string UsingStaticColor = "using static Teamcity.Host.Color;";

        public string Name => string.Empty;
        
        public bool Internal => true;

        public IEnumerator<string> GetEnumerator()
        {
            var lines = new List<string>() { LoadHost, UsingHost, UsingStaticHost, UsingStaticColor };
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}