// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public record CommandLine(
        string ExecutablePath,
        string WorkingDirectory,
        IReadOnlyCollection<string> Args,
        IReadOnlyCollection<(string, string)> Vars)
    {
        public CommandLine(string executablePath, params string[] args)
            :this(executablePath, string.Empty, args, ImmutableList<(string, string)>.Empty) { }
    }
}