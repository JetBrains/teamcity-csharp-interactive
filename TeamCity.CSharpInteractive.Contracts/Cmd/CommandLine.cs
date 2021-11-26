// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable CheckNamespace
namespace Cmd
{
    using System.Collections.Generic;
    using System.Linq;

    [Immutype.TargetAttribute]
    public record CommandLine(
        string ExecutablePath,
        string WorkingDirectory,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars)
    {
        public CommandLine(string executablePath, params string[] args)
            :this(executablePath, string.Empty, args, Enumerable.Empty<(string name, string value)>()) { }
    }
}