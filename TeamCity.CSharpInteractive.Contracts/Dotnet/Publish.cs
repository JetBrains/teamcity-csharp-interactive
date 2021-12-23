// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
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
        bool Integration = true,
        string ShortName = "")
        : IProcess
    {
        private readonly string _shortName = ShortName;

        public Publish()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty)
        { }
        
        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : "dotnet pack";

        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(ExecutablePath)
                .WithArgs("publish")
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddMSBuildIntegration(Integration, Verbosity)
                .AddArgs(
                    ("--output", Output),
                    ("--manifest", Manifest),
                    ("--framework", Framework),
                    ("--runtime", Runtime),
                    ("--configuration", Configuration),
                    ("--version-suffix", VersionSuffix),
                    ("--arch", Arch),
                    ("--os", OS)
                )
                .AddBooleanArgs(
                    ("--use-current-runtime", UseCurrentRuntime),
                    ("--no-build", NoBuild),
                    ("--self-contained", SelfContained),
                    ("--no-self-contained", NoSelfContained),
                    ("--nologo", NoLogo),
                    ("--no-restore", NoRestore)
                )
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}