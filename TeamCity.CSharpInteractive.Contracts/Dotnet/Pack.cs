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
        bool Integration = true,
        string ShortName = "")
        : IProcess
    {
        private readonly string _shortName = ShortName;

        public Pack()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty)
        { }
        
        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : "dotnet pack";

        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(ExecutablePath)
                .WithArgs("pack")
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddMSBuildIntegration(Integration, Verbosity)
                .AddArgs(
                    ("--output", Output),
                    ("--version-suffix", VersionSuffix),
                    ("--configuration", Configuration)
                )
                .AddBooleanArgs(
                    ("--no-build", NoBuild),
                    ("--include-symbols", IncludeSymbols),
                    ("--include-source", IncludeSource),
                    ("--serviceable", Serviceable),
                    ("--nologo", NoLogo),
                    ("--no-restore", NoRestore),
                    ("--use-current-runtime", UseCurrentRuntime)
                )
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}