// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;

    public record CommandLine(
        string ExecutablePath,
        string WorkingDirectory,
        IReadOnlyCollection<string> Args,
        IReadOnlyCollection<(string, string)> Vars)
    {
        public CommandLine(string executablePath, params string[] args)
            :this(executablePath, string.Empty, args, ImmutableList<(string, string)>.Empty)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(WorkingDirectory))
            {
                sb.Append(WorkingDirectory);
                sb.Append(" => ");
            }

            sb.Append('[');
            sb.Append(Escape(ExecutablePath));
            foreach (var arg in Args)
            {
                sb.Append(' ');
                sb.Append(Escape(arg));
            }
            sb.Append(']');

            // ReSharper disable once InvertIf
            if (Vars.Any())
            {
                sb.Append(" with environment variables ");
                foreach (var (name, value) in Vars)
                {
                    sb.Append('[');
                    sb.Append(name);
                    sb.Append('=');
                    sb.Append(value);
                    sb.Append(']');
                }
            }

            return sb.ToString();
        }

        private static string Escape(string text) => text.Contains(' ') ? $"\"{text}\"" : text;
    }
}