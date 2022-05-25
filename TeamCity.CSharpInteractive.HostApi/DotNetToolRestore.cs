// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

/// <summary>
/// The dotnet tool restore command finds the tool manifest file that is in scope for the current directory and installs the tools that are listed in it.
/// </summary>
[Target]
public record DotNetToolRestore(
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Adds an additional NuGet package source to use during installation. Feeds are accessed in parallel, not sequentially in some order of precedence. If the same package and version is in multiple feeds, the fastest feed wins.
    IEnumerable<string> AdditionalSources,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Prevent restoring multiple projects in parallel.
    bool? DisableParallel = default,
    // The NuGet configuration file (nuget.config) to use. If specified, only the settings from this file will be used. If not specified, the hierarchy of configuration files from the current directory will be used.
    string ConfigFile = "",
    // Path to the manifest file.
    string ToolManifest = "",
    // Do not cache packages and http requests.
    bool? NoCache = default,
    // Treat package source failures as warnings.
    bool? IgnoreFailedSources = default,
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
    : ICommandLine
{
    public DotNetToolRestore(params string[] args)
        : this(args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("tool", "restore")
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildLoggers(host, Verbosity)
            .AddArgs(AdditionalSources.Select(i => ("--add-source", (string?)i)).ToArray())
            .AddArgs(
                ("--configfile", ConfigFile),
                ("--tool-manifest", ToolManifest)
            )
            .AddBooleanArgs(
                ("--disable-parallel", DisableParallel),
                ("--no-cache", NoCache),
                ("--ignore-failed-sources", IgnoreFailedSources)
            )
            .AddArgs(Args.ToArray());

    public override string ToString() => "dotnet tool restore".GetShortName(ShortName, ToolManifest);
}