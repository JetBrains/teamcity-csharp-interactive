// ReSharper disable CheckNamespace
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.TargetAttribute]
    public record Custom(
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        bool Integration = false)
    {
        public Custom(params string[] args)
            : this(args, ImmutableList<(string, string)>.Empty)
        {
        }

        public static implicit operator CommandLine(Custom it) =>
            new CommandLine(it.ExecutablePath)
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .WithArgs(it.Args);
    }
}