// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Host;

    internal class HostIntegrationCodeSource: ICodeSource
    {
        private readonly IEnvironment _environment;
        private readonly ISession _session;
        internal const string UsingSystem = "using System;";
        internal const string UsingHost = "using Teamcity.Host;";
        internal const string UsingStaticHost = "using static Teamcity.Host.Host;";
        internal const string UsingStaticColor = "using static Teamcity.Host.Color;";

        public HostIntegrationCodeSource(IEnvironment environment, ISession session)
        {
            _environment = environment;
            _session = session;
        }

        public string Name => string.Empty;
        
        public bool Internal => true;

        public IEnumerator<string> GetEnumerator()
        {
            var lines = new List<string>
            {
                $"#r \"{Path.Combine(_environment.GetPath(SpecialFolder.Bin), "Teamcity.Host.dll")}\"",
                UsingSystem + UsingHost + UsingStaticHost + UsingStaticColor + $"{nameof(Host.ScriptInternal_SetPort)}({_session.Port});"
            };
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}