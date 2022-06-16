// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace HostApi;

using DotNet;
using Immutype;

/// <summary>
/// The dotnet build command builds the project and its dependencies into a set of binaries. The binaries include the project's code in Intermediate Language (IL) files with a .dll extension. Depending on the project type and settings, other files may be included.
/// </summary>
[Target]
public partial record DotNetBuild(
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
    // The project or solution file to build. If a project or solution file isn't specified, MSBuild searches the current working directory for a file that has a file extension that ends in either proj or sln and uses that file.
    string Project = "",
    // Directory in which to place the built binaries. If not specified, the default path is ./bin/<configuration>/<framework>/. For projects with multiple target frameworks (via the TargetFrameworks property), you also need to define --framework when you specify this option.
    string Output = "",
    // Compiles for a specific framework. The framework must be defined in the project file.
    string Framework = "",
    // Defines the build configuration. The default for most projects is Debug, but you can override the build configuration settings in your project.
    string Configuration = "",
    // Specifies the target runtime. For a list of Runtime Identifiers (RIDs), see the RID catalog. If you use this option with .NET 6 SDK, use --self-contained or --no-self-contained also.
    string Runtime = "",
    // Sets the value of the $(VersionSuffix) property to use when building the project. This only works if the $(Version) property isn't set. Then, $(Version) is set to the $(VersionPrefix) combined with the $(VersionSuffix), separated by a dash.
    string VersionSuffix = "",
    // Marks the build as unsafe for incremental build. This flag turns off incremental compilation and forces a clean rebuild of the project's dependency graph.
    bool? NoIncremental = default,
    // Ignores project-to-project (P2P) references and only builds the specified root project.
    bool? NoDependencies = default,
    // Doesn't display the startup banner or the copyright message. Available since .NET Core 3.0 SDK.
    bool? NoLogo = default,
    // Doesn't execute an implicit restore during build.
    bool? NoRestore = default,
    // Forces all dependencies to be resolved even if the last restore was successful. Specifying this flag is the same as deleting the project.assets.json file.
    bool? Force = default,
    // Publishes the .NET runtime with the application so the runtime doesn't need to be installed on the target machine. The default is true if a runtime identifier is specified. Available since .NET 6 SDK.
    bool? SelfContained = default,
    // Publishes the application as a framework dependent application. A compatible .NET runtime must be installed on the target machine to run the application. Available since .NET 6 SDK.
    bool? NoSelfContained = default,
    // Specifies the target architecture. This is a shorthand syntax for setting the Runtime Identifier (RID), where the provided value is combined with the default RID. For example, on a win-x64 machine, specifying --arch x86 sets the RID to win-x86. If you use this option, don't use the -r|--runtime option. Available since .NET 6 Preview 7.
    string Arch = "",
    // Specifies the target operating system (OS). This is a shorthand syntax for setting the Runtime Identifier (RID), where the provided value is combined with the default RID. For example, on a win-x64 machine, specifying --os linux sets the RID to linux-x64. If you use this option, don't use the -r|--runtime option. Available since .NET 6.
    string OS = "",
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
{
    public DotNetBuild(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("build")
            .AddNotEmptyArgs(Project)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildLoggers(host, Verbosity)
            .AddArgs(Sources.Select(i => ("--source", (string?)i)).ToArray())
            .AddArgs(
                ("--output", Output),
                ("--framework", Framework),
                ("--configuration", Configuration),
                ("--runtime", Runtime),
                ("--version-suffix", VersionSuffix),
                ("--verbosity", Verbosity?.ToString().ToLowerInvariant()),
                ("--arch", Arch),
                ("--os", OS)
            )
            .AddBooleanArgs(
                ("--no-incremental", NoIncremental),
                ("--no-dependencies", NoDependencies),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore),
                ("--self-contained", SelfContained),
                ("--no-self-contained", NoSelfContained),
                ("--force", Force)
            )
            .AddProps("-p", Props.ToArray())
            .AddArgs(Args.ToArray());
    
    public override string ToString() => "dotnet build".GetShortName(ShortName, Project);
}