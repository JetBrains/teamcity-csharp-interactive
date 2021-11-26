// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.TargetAttribute]
    public record Publish(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Project = "",
        bool UseCurrentRuntime = false,
        string Output = "",
        string Manifest = "",
        bool NoBuild = false,
        bool SelfContained = false,
        bool NoSelfContained = false,
        bool NoLogo = false,
        string Framework = "",
        string Runtime = "",
        string Configuration = "",
        string VersionSuffix = "",
        bool NoRestore = false,
        string Arch = "",
        string OS = "",
        Verbosity? Verbosity = default,
        bool Integration = true)
    {
        public Publish()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty)
        {
        }

        public static implicit operator CommandLine(Publish it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("publish")
            .AddArgs(new []{
                it.Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddMSBuildIntegration(it.Integration, it.Verbosity)
            .AddArgs(
                ("--output", it.Output),
                ("--manifest", it.Manifest),
                ("--framework", it.Framework),
                ("--runtime", it.Runtime),
                ("--configuration", it.Configuration),
                ("--version-suffix", it.VersionSuffix),
                ("--arch", it.Arch),
                ("--os", it.OS)
            )
            .AddBooleanArgs(
                ("--use-current-runtime", it.UseCurrentRuntime),
                ("--no-build", it.NoBuild),
                ("--self-contained", it.SelfContained),
                ("--no-self-contained", it.NoSelfContained),
                ("--nologo", it.NoLogo),
                ("--no-restore", it.NoRestore)
            )
            .AddProps("/p", it.Props.ToArray())
            .AddArgs(it.Args.ToArray());
    }
}