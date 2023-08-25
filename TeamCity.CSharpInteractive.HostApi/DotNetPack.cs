// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;
using JetBrains.TeamCity.ServiceMessages;

/// <summary>
/// The dotnet pack command builds the project and creates NuGet packages. The result of this command is a NuGet package (that is, a .nupkg file).
/// </summary>
[Target]
public partial record DotNetPack(
    // MSBuild options for setting properties.
    IEnumerable<(string name, string value)> Props,
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    string Project = "",
    // Places the built packages in the directory specified.
    string Output = "", 
    // Specifies the target runtime to restore packages for. For a list of Runtime Identifiers (RIDs), see the RID catalog.
    string Runtime = "",
    // Doesn't build the project before packing. It also implicitly sets the --no-restore flag.
    bool? NoBuild = default,
    // Ignores project-to-project references and only restores the root project.
    bool? NoDependencies = default,
    // Includes the debug symbols NuGet packages in addition to the regular NuGet packages in the output directory.
    bool? IncludeSymbols = default,
    // Includes the debug symbols NuGet packages in addition to the regular NuGet packages in the output directory. The sources files are included in the src folder within the symbols package.
    bool? IncludeSource = default,
    // Sets the serviceable flag in the package. For more information, see .NET Blog: .NET Framework 4.5.1 Supports Microsoft Security Updates for .NET NuGet Libraries.
    bool? Serviceable = default,
    // Doesn't display the startup banner or the copyright message. Available since .NET Core 3.0 SDK.
    bool? NoLogo = default,
    // Doesn't execute an implicit restore when running the command.
    bool? NoRestore = default,
    // Defines the value for the VersionSuffix MSBuild property. The effect of this property on the package version depends on the values of the Version and VersionPrefix properties.
    string VersionSuffix = "",
    // Defines the build configuration. The default for most projects is Debug, but you can override the build configuration settings in your project.
    string Configuration = "",
    bool? UseCurrentRuntime = default,
    // Forces all dependencies to be resolved even if the last restore was successful. Specifying this flag is the same as deleting the project.assets.json file.
    bool? Force = default,
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
{
    public DotNetPack(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("pack")
            .AddNotEmptyArgs(Project)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildLoggers(host, Verbosity)
            .AddTeamCityEnvironmentVariables(host)
            .AddArgs(
                ("--output", Output),
                ("--version-suffix", VersionSuffix),
                ("--configuration", Configuration),
                ("--runtime", Runtime)
            )
            .AddBooleanArgs(
                ("--no-build", NoBuild),
                ("--no-dependencies", NoDependencies),
                ("--include-symbols", IncludeSymbols),
                ("--include-source", IncludeSource),
                ("--serviceable", Serviceable),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore),
                ("--use-current-runtime", UseCurrentRuntime),
                ("--force", Force)
            )
            .AddProps("-p", Props.ToArray())
            .AddArgs(Args.ToArray());

    public void PreRun(IHost host) => host.GetService<IDotNetTestReportingService>().SendTestResultsStreamingDataMessageIfNeeded();

    public IEnumerable<IServiceMessage> GetNonStdStreamsServiceMessages(IHost host) =>
        host.GetService<IDotNetTestReportingService>().GetServiceMessagesFromFilesWithTestReports();

    public override string ToString() => "dotnet pack".GetShortName(ShortName, Project);
}
