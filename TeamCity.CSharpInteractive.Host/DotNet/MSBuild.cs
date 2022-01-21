// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace DotNet;

using Cmd;
using Script;

[Immutype.Target]
public record MSBuild(
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Props,
    IEnumerable<(string name, string value)> Vars,
    IEnumerable<(string name, string value)> RestoreProps,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Project = "",
    string Target = "",
    int? MaxCpuCount = default,
    string ToolsVersion = "",
    string WarnAsError = "",
    string WarnAsMessage = "",
    string IgnoreProjectExtensions = "",
    bool? NodeReuse = default,
    string Preprocess = "",
    bool DetailedSummary = false,
    bool? Restore = default,
    string ProfileEvaluation = "",
    bool? IsolateProjects = default,
    string InputResultsCaches = "",
    string OutputResultsCache = "",
    bool? GraphBuild = default,
    bool? LowPriority = default,
    bool NoAutoResponse = false,
    bool NoLogo = false,
    bool Version = false,
    Verbosity? Verbosity = default,
    string ShortName = "")
    : IProcess
{
    public MSBuild()
        : this(Enumerable.Empty<string>(), Enumerable.Empty<(string, string)>(), Enumerable.Empty<(string, string)>(), Enumerable.Empty<(string, string)>())
    { }
        
    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<ISettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : ExecutablePath == string.Empty ? "dotnet msbuild" : Path.GetFileNameWithoutExtension(ExecutablePath))
            .WithArgs(ExecutablePath == string.Empty ? new [] {"msbuild"} : Array.Empty<string>())
            .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
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
                ("-version", Version)
            )
            .AddProps("-restoreProperty", RestoreProps.ToArray())
            .AddProps("/p", Props.ToArray())
            .AddArgs(Args.ToArray());
}