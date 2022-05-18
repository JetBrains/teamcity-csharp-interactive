// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace HostApi;

using DotNet;
using Immutype;

/// <summary>
/// The dotnet run command provides a convenient option to run your application from the source code with one command. It's useful for fast iterative development from the command line. The command depends on the dotnet build command to build the code. Any requirements for the build, such as that the project must be restored first, apply to dotnet run as well.
/// </summary>
[Target]
public record DotNetRun(
    // MSBuild options for setting properties.
    IEnumerable<(string name, string value)> Props,
    // Specifies the set of command line arguments to use when starting the application.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Builds and runs the app using the specified framework. The framework must be specified in the project file.
    string Framework = "",
    // Defines the build configuration. The default for most projects is Debug, but you can override the build configuration settings in your project.
    string Configuration = "",
    // Specifies the target runtime to restore packages for. For a list of Runtime Identifiers (RIDs), see the RID catalog. -r short option available since .NET Core 3.0 SDK.
    string Runtime = "",
    // Specifies the path of the project file to run (folder name or full path). If not specified, it defaults to the current directory.
    string Project = "",
    // The name of the launch profile (if any) to use when launching the application. Launch profiles are defined in the launchSettings.json file and are typically called Development, Staging, and Production. For more information, see Working with multiple environments.
    string LaunchProfile = "",
    // Doesn't try to use launchSettings.json to configure the application.
    bool? NoLaunchProfile = default,
    // Doesn't build the project before running. It also implicit sets the --no-restore flag.
    bool? NoBuild = default,
    // Doesn't execute an implicit restore when running the command.
    bool? NoRestore = default,
    // When restoring a project with project-to-project (P2P) references, restores the root project and not the references.
    bool? NoDependencies = default,
    // Forces all dependencies to be resolved even if the last restore was successful. Specifying this flag is the same as deleting the project.assets.json file.
    bool? Force = default,
    // Specifies the target architecture. This is a shorthand syntax for setting the Runtime Identifier (RID), where the provided value is combined with the default RID. For example, on a win-x64 machine, specifying --arch x86 sets the RID to win-x86. If you use this option, don't use the -r|--runtime option. Available since .NET 6 Preview 7.
    string Arch = "",
    // Specifies the target operating system (OS). This is a shorthand syntax for setting the Runtime Identifier (RID), where the provided value is combined with the default RID. For example, on a win-x64 machine, specifying --os linux sets the RID to linux-x64. If you use this option, don't use the -r|--runtime option. Available since .NET 6.
    string OS = "",
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
    : ICommandLine
{
    public DotNetRun(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("run")
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddArgs(
                ("--framework", Framework),
                ("--configuration", Configuration),
                ("--runtime", Runtime),
                ("--project", Project),
                ("--launch-profile", LaunchProfile),
                ("--verbosity", Verbosity?.ToString().ToLowerInvariant()),
                ("--arch", Arch),
                ("--os", OS)
            )
            .AddBooleanArgs(
                ("--no-launch-profile", NoLaunchProfile),
                ("--no-build", NoBuild),
                ("--no-restore", NoRestore),
                ("--no-dependencies", NoDependencies),
                ("--force", Force)
            )
            .AddProps("--property", Props.ToArray())
            .AddArgs(Args.ToArray());

    public override string ToString() => "dotnet run".GetShortName(ShortName, Project);
}