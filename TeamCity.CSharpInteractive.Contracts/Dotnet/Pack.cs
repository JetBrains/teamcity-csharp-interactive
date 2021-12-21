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
    public record Pack(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Project = "",
        string Output = "",
        bool NoBuild = false,
        bool IncludeSymbols = false,
        bool IncludeSource = false,
        bool Serviceable = false,
        bool NoLogo = false,
        bool NoRestore = false,
        string VersionSuffix = "",
        string Configuration = "",
        bool UseCurrentRuntime = false,
        Verbosity? Verbosity = default,
        bool Integration = true)
    {
        public Pack()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty)
        { }

        public static implicit operator CommandLine(Pack it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("pack")
            .AddArgs(new []{
                it.Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddMSBuildIntegration(it.Integration, it.Verbosity)
            .AddArgs(
                ("--output", it.Output),
                ("--version-suffix", it.VersionSuffix),
                ("--configuration", it.Configuration)
            )
            .AddBooleanArgs(
                ("--no-build", it.NoBuild),
                ("--include-symbols", it.IncludeSymbols),
                ("--include-source", it.IncludeSource),
                ("--serviceable", it.Serviceable),
                ("--nologo", it.NoLogo),
                ("--no-restore", it.NoRestore),
                ("--use-current-runtime", it.UseCurrentRuntime)
            )
            .AddProps("/p", it.Props.ToArray())
            .AddArgs(it.Args.ToArray());
    }
}