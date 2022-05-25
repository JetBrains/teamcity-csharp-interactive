// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

/// <summary>
/// The dotnet restore command uses NuGet to restore dependencies as well as project-specific tools that are specified in the project file. In most cases, you don't need to explicitly use the dotnet restore command.
/// </summary>
[Target]
public record DotNetRestore(
    // MSBuild options for setting properties.
    IEnumerable<(string name, string value)> Props,
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Specifies the URI of the NuGet package source to use during the restore operation. This setting overrides all of the sources specified in the nuget.config files.
    IEnumerable<string> Sources,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Optional path to the project file to restore.
    string Project = "",
    // Specifies the directory for restored packages.
    string Packages = "",
    // Use current runtime as the target runtime.
    bool? UseCurrentRuntime = default,
    // Disables restoring multiple projects in parallel.
    bool? DisableParallel = default,
    // The NuGet configuration file (nuget.config) to use. If specified, only the settings from this file will be used. If not specified, the hierarchy of configuration files from the current directory will be used.
    string ConfigFile = "",
    // Specifies to not cache HTTP requests.
    bool? NoCache = default,
    // Only warn about failed sources if there are packages meeting the version requirement.
    bool? IgnoreFailedSources = default,
    // Forces all dependencies to be resolved even if the last restore was successful. Specifying this flag is the same as deleting the project.assets.json file.
    bool? Force = default,
    // Specifies a runtime for the package restore. This is used to restore packages for runtimes not explicitly listed in the <RuntimeIdentifiers> tag in the .csproj file. For a list of Runtime Identifiers (RIDs), see the RID catalog. Provide multiple RIDs by specifying this option multiple times.
    string Runtime = "",
    // When restoring a project with project-to-project (P2P) references, restores the root project and not the references.
    bool? NoDependencies = default,
    // Enables project lock file to be generated and used with restore.
    bool? UseLockFile = default,
    // Don't allow updating project lock file.
    bool? LockedMode = default,
    // Output location where project lock file is written. By default, this is PROJECT_ROOT\packages.lock.json.
    string LockFilePath = "",
    // Forces restore to reevaluate all dependencies even if a lock file already exists.
    bool? ForceEvaluate = default,
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
    : ICommandLine
{
    public DotNetRestore(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("restore")
            .AddNotEmptyArgs(Project)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildLoggers(host, Verbosity)
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
            .AddProps("-p", Props.ToArray())
            .AddArgs(Args.ToArray());

    public override string ToString() => "dotnet restore".GetShortName(ShortName, Project);
}