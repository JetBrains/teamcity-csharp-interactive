// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record Run(
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Framework = "",
        string Configuration = "",
        string Runtime = "",
        string Project = "",
        string LaunchProfile = "",
        bool NoLaunchProfile = false,
        bool NoBuild = false,
        bool NoRestore = false,
        bool NoDependencies = false,
        bool Force = false,
        Verbosity? Verbosity = default,
        bool Integration = true)
    {
        public Run()
            : this(ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty)
        { }

        public static implicit operator CommandLine(Run it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("run")
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddArgs(
                ("--framework", it.Framework),
                ("--configuration", it.Configuration),
                ("--runtime", it.Runtime),
                ("--project", it.Project),
                ("--launch-profile", it.LaunchProfile),
                ("--verbosity", it.Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--no-launch-profile", it.NoLaunchProfile),
                ("--no-build", it.NoBuild),
                ("--no-restore", it.NoRestore),
                ("--no-dependencies", it.NoDependencies),
                ("--force", it.Force)
            )
            .AddArgs(it.Args.ToArray());
    }
}