// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

/// <summary>
/// The dotnet publish command compiles the application, reads through its dependencies specified in the project file, and publishes the resulting set of files to a directory.
/// </summary>
[Target]
public partial record DotNetPublish(
    // MSBuild options for setting properties.
    IEnumerable<(string name, string value)> Props,
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // The URI of the NuGet package source to use during the restore operation.
    IEnumerable<string> Sources,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // The project or solution to publish.
    string Project = "",
    bool? UseCurrentRuntime = default,
    // Specifies the path for the output directory. If not specified, it defaults to [project_file_folder]/bin/[configuration]/[framework]/publish/ for a framework-dependent executable and cross-platform binaries. It defaults to [project_file_folder]/bin/[configuration]/[framework]/[runtime]/publish/ for a self-contained executable.In a web project, if the output folder is in the project folder, successive dotnet publish commands result in nested output folders.
    string Output = "",
    // Specifies one or several target manifests to use to trim the set of packages published with the app. The manifest file is part of the output of the dotnet store command. To specify multiple manifests, add a --manifest option for each manifest.
    string Manifest = "",
    // Doesn't build the project before publishing. It also implicitly sets the --no-restore flag.
    bool? NoBuild = default,
    // Ignores project-to-project references and only restores the root project.
    bool? NoDependencies = default,
    // Publishes the .NET runtime with your application so the runtime doesn't need to be installed on the target machine. Default is true if a runtime identifier is specified and the project is an executable project (not a library project). For more information, see .NET application publishing and Publish .NET apps with the .NET CLI. If this option is used without specifying true or false, the default is true. In that case, don't put the solution or project argument immediately after --self-contained, because true or false is expected in that position.
    bool? SelfContained = default,
    // Equivalent to --self-contained false. Available since .NET Core 3.0 SDK.
    bool? NoSelfContained = default,
    // Doesn't display the startup banner or the copyright message. Available since .NET Core 3.0 SDK.
    bool? NoLogo = default,
    // Forces all dependencies to be resolved even if the last restore was successful. Specifying this flag is the same as deleting the project.assets.json file.
    bool? Force = default,
    // Publishes the application for the specified target framework. You must specify the target framework in the project file.
    string Framework = "",
    // Publishes the application for a given runtime. For a list of Runtime Identifiers (RIDs), see the RID catalog. For more information, see .NET application publishing and Publish .NET apps with the .NET CLI. If you use this option, use --self-contained or --no-self-contained also.
    string Runtime = "",
    // Defines the build configuration. The default for most projects is Debug, but you can override the build configuration settings in your project.
    string Configuration = "",
    // Defines the version suffix to replace the asterisk (*) in the version field of the project file.
    string VersionSuffix = "",
    // Doesn't execute an implicit restore when running the command.
    bool? NoRestore = default,
    // Specifies the target architecture. This is a shorthand syntax for setting the Runtime Identifier (RID), where the provided value is combined with the default RID. For example, on a win-x64 machine, specifying --arch x86 sets the RID to win-x86. If you use this option, don't use the -r|--runtime option. Available since .NET 6 Preview 7.
    string Arch = "",
    // Specifies the target operating system (OS). This is a shorthand syntax for setting the Runtime Identifier (RID), where the provided value is combined with the default RID. For example, on a win-x64 machine, specifying --os linux sets the RID to linux-x64. If you use this option, don't use the -r|--runtime option. Available since .NET 6.
    string OS = "",
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
{
    public DotNetPublish(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("publish")
            .AddNotEmptyArgs(Project)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildLoggers(host, Verbosity)
            .AddArgs(Sources.Select(i => ("--source", (string?)i)).ToArray())
            .AddArgs(
                ("--output", Output),
                ("--manifest", Manifest),
                ("--framework", Framework),
                ("--runtime", Runtime),
                ("--configuration", Configuration),
                ("--version-suffix", VersionSuffix),
                ("--arch", Arch),
                ("--os", OS)
            )
            .AddBooleanArgs(
                ("--use-current-runtime", UseCurrentRuntime),
                ("--no-build", NoBuild),
                ("--no-dependencies", NoDependencies),
                ("--self-contained", SelfContained),
                ("--no-self-contained", NoSelfContained),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore),
                ("--force", Force)
            )
            .AddProps("-p", Props.ToArray())
            .AddArgs(Args.ToArray());

    public override string ToString() => "dotnet publish".GetShortName(ShortName, Project);
}