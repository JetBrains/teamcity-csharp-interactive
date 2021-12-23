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
    public record Restore(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        IEnumerable<string> Sources,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Project = "",
        string Packages = "",
        bool UseCurrentRuntime = false,
        bool DisableParallel = false,
        string ConfigFile = "",
        bool NoCache = false,
        bool IgnoreFailedSources = false,
        bool Force = false,
        string Runtime = "",
        bool NoDependencies = false,
        bool UseLockFile = false,
        bool LockedMode = false,
        string LockFilePath = "",
        bool ForceEvaluate = false,
        Verbosity? Verbosity = default,
        bool Integration = true,
        string ShortName = "")
        : IProcess
    {
        private readonly string _shortName = ShortName;

        public Restore()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty)
        { }
        
        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : "dotnet restore";

        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(ExecutablePath)
                .WithArgs("restore")
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddMSBuildIntegration(Integration, Verbosity)
                .AddArgs(Sources.Select(i => ("--source", (string?)i)).ToArray())
                .AddArgs(
                    ("--packages", Packages),
                    ("--configfile", ConfigFile),
                    ("--runtime", Runtime),
                    ("--lock-file-path", LockFilePath)
                )
                .AddBooleanArgs(
                    ("--use-current-runtime", UseCurrentRuntime),
                    ("--disable-parallel", DisableParallel),
                    ("--no-cache", NoCache),
                    ("--ignore-failed-sources", IgnoreFailedSources),
                    ("--force", Force),
                    ("--no-dependencies", NoDependencies),
                    ("--use-lock-file", UseLockFile),
                    ("--locked-mode", LockedMode),
                    ("--force-evaluate", ForceEvaluate)
                )
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}