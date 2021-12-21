// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable CheckNamespace
namespace Cmd
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Immutype.Target]
    public record CommandLine(
        string ExecutablePath,
        string WorkingDirectory,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars)
    {
        public CommandLine(string executablePath, params string[] args)
            :this(executablePath, string.Empty, args, Enumerable.Empty<(string name, string value)>())
        { }

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