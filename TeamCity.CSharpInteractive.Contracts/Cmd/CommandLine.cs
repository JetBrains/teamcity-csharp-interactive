// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable CheckNamespace
namespace Cmd
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record CommandLine(
        string ExecutablePath,
        string WorkingDirectory,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ShortName = "")
        : IStartInfo, IProcess
    {
        private readonly string _shortName = ShortName;

        public CommandLine(string executablePath, params string[] args)
            :this(executablePath, string.Empty, args, Enumerable.Empty<(string name, string value)>())
        { }

        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : Path.GetFileNameWithoutExtension(ExecutablePath);

        public IStartInfo GetStartInfo(IHost host) => this;

        public ProcessState GetState(int exitCode) => ProcessState.Unknown;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            sb.Append(Escape(ExecutablePath));
            foreach (var arg in Args)
            {
                sb.Append(' ');
                sb.Append(Escape(arg));
            }
            sb.Append(']');

            if (!string.IsNullOrWhiteSpace(WorkingDirectory))
            {
                sb.Append(" in ");
                sb.Append(Escape(WorkingDirectory));
            }

            // ReSharper disable once InvertIf
            if (Vars.Any())
            {
                sb.Append(" with variables [");
                var first = true;
                foreach (var (name, value) in Vars)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    
                    sb.Append(name);
                    sb.Append('=');
                    sb.Append(Escape(value));
                }
                sb.Append(']');
            }

            return sb.ToString();
        }
        
        private static string Escape(string text) => !text.TrimStart().StartsWith("\"") && text.Contains(' ') ? $"\"{text}\"" : text;
    }
}