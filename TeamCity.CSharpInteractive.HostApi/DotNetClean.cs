// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

[Target]
public record DotNetClean(
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
    // The MSBuild project or solution to clean. If a project or solution file is not specified, MSBuild searches the current working directory for a file that has a file extension that ends in proj or sln, and uses that file.
    string Project = "",
    // The framework that was specified at build time. The framework must be defined in the project file. If you specified the framework at build time, you must specify the framework when cleaning.
    string Framework = "",
    // Cleans the output folder of the specified runtime. This is used when a self-contained deployment was created.
    string Runtime = "",
    // Defines the build configuration. The default for most projects is Debug, but you can override the build configuration settings in your project. This option is only required when cleaning if you specified it during build time.
    string Configuration = "",
    // The directory that contains the build artifacts to clean. Specify the -f|--framework <FRAMEWORK> switch with the output directory switch if you specified the framework when the project was built.
    string Output = "",
    // Doesn't display the startup banner or the copyright message. Available since .NET Core 3.0 SDK.
    bool? NoLogo = default,
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
    : ICommandLine
{
    public DotNetClean(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("clean")
            .AddNotEmptyArgs(Project)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildIntegration(host, Verbosity)
            .AddArgs(
                ("--output", Output),
                ("--framework", Framework),
                ("--runtime", Runtime),
                ("--configuration", Configuration),
                ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--nologo", NoLogo)
            )
            .AddProps("/p", Props.ToArray())
            .AddArgs(Args.ToArray());

    public override string ToString() => "dotnet clean".GetShortName(ShortName, Project);
}