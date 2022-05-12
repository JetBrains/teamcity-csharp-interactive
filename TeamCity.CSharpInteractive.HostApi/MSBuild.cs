// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace HostApi;

using DotNet;
using Immutype;

[Target]
public record MSBuild(
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Set or override these project-level properties.
    IEnumerable<(string name, string value)> Props,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    IEnumerable<(string name, string value)> RestoreProps,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Builds the specified targets in the project file. If a project file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in "proj" and uses that file. If a directory is specified, MSBuild searches that directory for a project file.
    string Project = "",
    // Build these targets in this project. Use a semicolon or a comma to separate multiple targets, or specify each target separately.
    string Target = "",
    // Specifies the maximum number of concurrent processes to build with. If the switch is not used, the default value used is 1. If the switch is used without a value MSBuild will use up to the number of processors on the computer.
    int? MaxCpuCount = default,
    // The version of the MSBuild Toolset (tasks, targets, etc.) to use during build. This version will override the versions specified by individual projects.
    string ToolsVersion = "",
    // List of warning codes to treats as errors. Use a semicolon or a comma to separate multiple warning codes. To treat all warnings as errors use the switch with no values.
    string WarnAsError = "",
    // List of warning codes to treats as low importance messages. Use a semicolon or a comma to separate multiple warning codes.
    string WarnAsMessage = "",
    // List of extensions to ignore when determining which project file to build. Use a semicolon or a comma to separate multiple extensions.
    string IgnoreProjectExtensions = "",
    // Enables or Disables the reuse of MSBuild nodes.
    bool? NodeReuse = default,
    // Creates a single, aggregated project file by inlining all the files that would be imported during a build, with their boundaries marked. This can be useful for figuring out what files are being imported and from where, and what they will contribute to the build. By default the output is written to the console window. If the path to an output file is provided that will be used instead.
    string Preprocess = "",
    // Shows detailed information at the end of the build about the configurations built and how they were scheduled to nodes.
    bool DetailedSummary = false,
    // Runs a target named Restore prior to building other targets and ensures the build for these targets uses the latest restored build logic. This is useful when your project tree requires packages to be restored before it can be built.
    bool? Restore = default,
    // Profiles MSBuild evaluation and writes the result to the specified file. If the extension of the specified file is '.md', the result is generated in markdown format. Otherwise, a tab separated file is produced.
    string ProfileEvaluation = "",
    // Causes MSBuild to build each project in isolation. This is a more restrictive mode of MSBuild as it requires that the project graph be statically discoverable at evaluation time, but can improve scheduling and reduce memory overhead when building a large set of projects.
    bool? IsolateProjects = default,
    // Semicolon separated list of input cache files that MSBuild will read build results from. Setting this also turns on isolated builds.
    string InputResultsCaches = "",
    // Output cache file where MSBuild will write the contents of its build result caches at the end of the build. Setting this also turns on isolated builds.
    string OutputResultsCache = "",
    // Causes MSBuild to construct and build a project graph. Constructing a graph involves identifying project references to form dependencies. Building that graph involves attempting to build project references prior to the projects that reference them, differing from traditional MSBuild scheduling.
    bool? GraphBuild = default,
    // Causes MSBuild to run at low process priority.
    bool? LowPriority = default,
    // Do not auto-include any MSBuild.rsp files.
    bool? NoAutoResponse = default,
    // Do not display the startup banner and copyright message.
    bool? NoLogo = default,
    // Display version information only.
    bool? DisplayVersion = default,
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
    : ICommandLine
{
    public MSBuild()
        : this(Enumerable.Empty<string>(), Enumerable.Empty<(string, string)>(), Enumerable.Empty<(string, string)>(), Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(ToString())
            .WithArgs(ExecutablePath == string.Empty ? new[] {"msbuild"} : Array.Empty<string>())
            .AddArgs(new[] {Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildIntegration(host, Verbosity)
            .AddMSBuildArgs(
                ("-target", Target),
                ("-maxCpuCount", MaxCpuCount?.ToString()),
                ("-toolsVersion", ToolsVersion),
                ("-verbosity", Verbosity?.ToString().ToLower()),
                ("-warnAsError", WarnAsError),
                ("-warnAsMessage", WarnAsMessage),
                ("-ignoreProjectExtensions", IgnoreProjectExtensions),
                ("-nodeReuse", NodeReuse?.ToString()),
                ("-preprocess", Preprocess),
                ("-restore", Restore?.ToString()),
                ("-profileEvaluation", ProfileEvaluation),
                ("-isolateProjects", IsolateProjects?.ToString()),
                ("-inputResultsCaches", InputResultsCaches),
                ("-outputResultsCache", OutputResultsCache),
                ("-graphBuild", GraphBuild?.ToString()),
                ("-lowPriority", LowPriority?.ToString())
            )
            .AddBooleanArgs(
                ("-detailedSummary", DetailedSummary),
                ("-noAutoResponse", NoAutoResponse),
                ("-noLogo", NoLogo),
                ("-version", DisplayVersion)
            )
            .AddProps("-restoreProperty", RestoreProps.ToArray())
            .AddProps("-p", Props.ToArray())
            .AddArgs(Args.ToArray());
    
    public override string ToString() => (ExecutablePath == string.Empty ? "dotnet msbuild" : Path.GetFileNameWithoutExtension(ExecutablePath)).GetShortName(ShortName, Project);
}