// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.TargetAttribute]
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
        bool Integration = true)
    {
        public Restore()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty)
        {
        }

        public static implicit operator CommandLine(Restore it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("restore")
            .AddArgs(new []{
                it.Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddMSBuildIntegration(it.Integration, it.Verbosity)
            .AddArgs(it.Sources.Select(i => ("--source", (string?)i)).ToArray())
            .AddArgs(
                ("--packages", it.Packages),
                ("--configfile", it.ConfigFile),
                ("--runtime", it.Runtime),
                ("--lock-file-path", it.LockFilePath)
            )
            .AddBooleanArgs(
                ("--use-current-runtime", it.UseCurrentRuntime),
                ("--disable-parallel", it.DisableParallel),
                ("--no-cache", it.NoCache),
                ("--ignore-failed-sources", it.IgnoreFailedSources),
                ("--force", it.Force),
                ("--no-dependencies", it.NoDependencies),
                ("--use-lock-file", it.UseLockFile),
                ("--locked-mode", it.LockedMode),
                ("--force-evaluate", it.ForceEvaluate)
            )
            .AddProps("/p", it.Props.ToArray())
            .AddArgs(it.Args.ToArray());
    }
}