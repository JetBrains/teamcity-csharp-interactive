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
    public record Build(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Project = "",
        string Output = "",
        string Framework = "",
        string Configuration = "",
        string Runtime = "",
        string VersionSuffix = "",
        bool NoIncremental = false,
        bool NoDependencies = false,
        bool NoLogo = false,
        bool NoRestore = false,
        bool Force = false,
        Verbosity? Verbosity = default,
        bool Integration = true)
    {
        public Build()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList<string>.Empty, ImmutableList<(string, string)>.Empty)
        { }

        public static implicit operator CommandLine(Build it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("build")
            .AddArgs(new []{
                it.Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddMSBuildIntegration(it.Integration, it.Verbosity)
            .AddArgs(
                ("--output", it.Output),
                ("--framework", it.Framework),
                ("--configuration", it.Configuration),
                ("--runtime", it.Runtime),
                ("--version-suffix", it.VersionSuffix),
                ("--verbosity", it.Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--no-incremental", it.NoIncremental),
                ("--no-dependencies", it.NoDependencies),
                ("--nologo", it.NoLogo),
                ("--no-restore", it.NoRestore),
                ("--force", it.Force)
            )
            .AddProps("/p", it.Props.ToArray())
            .AddArgs(it.Args.ToArray());
    }
}