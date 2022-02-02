// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

[Target]
public record DotNetRestore(
    IEnumerable<(string name, string value)> Props,
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    IEnumerable<string> Sources,
    string ExecutablePath = "",
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
    DotNetVerbosity? Verbosity = default,
    string ShortName = "")
    : ICommandLine
{
    public DotNetRestore(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("restore")
            .AddNotEmptyArgs(Project)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildIntegration(host, Verbosity)
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

    public override string ToString() => "dotnet restore".GetShortName(ShortName, Project);
}