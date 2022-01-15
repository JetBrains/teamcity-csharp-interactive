// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable CheckNamespace
// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace Cmd
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    [DebuggerTypeProxy(typeof(StartInfoDebugView))]
    public record CommandLine(
        string ExecutablePath,
        string WorkingDirectory,
        IReadOnlyList<string> Args,
        IReadOnlyList<(string name, string value)> Vars,
        string ShortName = "")
        : IStartInfo, IProcess
    {
        private readonly string _shortName = ShortName;

        public CommandLine(string executablePath, params string[] args)
            :this(executablePath, string.Empty, args, ImmutableArray<(string name, string value)>.Empty)
        { }

        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : Path.GetFileNameWithoutExtension(ExecutablePath);

        public IStartInfo GetStartInfo(IHost host) => this;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(ShortName))
            {
                sb.Append(ShortName);
                sb.Append(": ");
            }

            sb.Append(Escape(ExecutablePath));
            foreach (var arg in Args)
            {
                sb.Append(' ');
                sb.Append(Escape(arg));
            }
            
            return sb.ToString();
        }
        
        private static string Escape(string text) => !text.TrimStart().StartsWith("\"") && text.Contains(' ') ? $"\"{text}\"" : text;
        
        internal class StartInfoDebugView
        {
            private readonly IStartInfo _startInfo;

            public StartInfoDebugView(IStartInfo startInfo) => _startInfo = startInfo;

            public string ShortName => _startInfo.ShortName;

            public string ExecutablePath => _startInfo.ExecutablePath;

            public string WorkingDirectory => _startInfo.WorkingDirectory;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<string> Args => _startInfo.Args;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<(string name, string value)> Vars => _startInfo.Vars;
        }
    }
}