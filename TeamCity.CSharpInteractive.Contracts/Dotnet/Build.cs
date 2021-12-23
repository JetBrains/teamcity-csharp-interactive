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
        bool Integration = true,
        string ShortName = "")
        : IProcess
    {
        private readonly string _shortName = ShortName;

        public Build()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList<string>.Empty, ImmutableList<(string, string)>.Empty)
        { }

        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : "dotnet build";

        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(ExecutablePath)
                .WithArgs("build")
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddMSBuildIntegration(Integration, Verbosity)
                .AddArgs(
                    ("--output", Output),
                    ("--framework", Framework),
                    ("--configuration", Configuration),
                    ("--runtime", Runtime),
                    ("--version-suffix", VersionSuffix),
                    ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
                )
                .AddBooleanArgs(
                    ("--no-incremental", NoIncremental),
                    ("--no-dependencies", NoDependencies),
                    ("--nologo", NoLogo),
                    ("--no-restore", NoRestore),
                    ("--force", Force)
                )
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}