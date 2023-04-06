// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace HostApi;

using Cmd;
using DotNet;
using Immutype;

/// <summary>
/// The dotnet test command is used to execute unit tests in a given solution. The dotnet test command builds the solution and runs a test host application for each test project in the solution. The test host executes tests in the given project using a test framework, for example: MSTest, NUnit, or xUnit, and reports the success or failure of each test. If all tests are successful, the test runner returns 0 as an exit code; otherwise if any test fails, it returns 1.
/// </summary>
[Target]
public partial record DotNetTest(
    // MSBuild options for setting properties.
    IEnumerable<(string name, string value)> Props,
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Specifies RunSettings passed to tests.
    IEnumerable<(string name, string value)> RunSettings,
    // Specifies a logger for test results. Use "console;verbosity=detailed". Specify the parameter multiple times to enable multiple loggers.
    IEnumerable<string> Loggers,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Path to the test project, to the solution, to a directory that contains a project or a solution or to a test project .dll file.
    string Project = "",
    // The .runsettings file to use for running the tests. The TargetPlatform element (x86|x64) has no effect for dotnet test. To run tests that target x86, install the x86 version of .NET Core. The bitness of the dotnet.exe that is on the path is what will be used for running tests.
    string Settings = "",
    // List the discovered tests instead of running the tests.
    bool? ListTests = default,
    // Filters out tests in the current project using the given expression. For more information, see the Filter option details section. For more information and examples on how to use selective unit test filtering, see Running selective unit tests.
    string Filter = "",
    // Path to a directory to be searched for additional test adapters. Only .dll files with suffix .TestAdapter.dll are inspected. If not specified, the directory of the test .dll is searched.
    string TestAdapterPath = "",
    // Defines the build configuration. The default for most projects is Debug, but you can override the build configuration settings in your project.
    string Configuration = "",
    // Forces the use of dotnet or .NET Framework test host for the test binaries. This option only determines which type of host to use. The actual framework version to be used is determined by the runtimeconfig.json of the test project. When not specified, the TargetFramework assembly attribute is used to determine the type of host. When that attribute is stripped from the .dll, the .NET Framework host is used.
    string Framework = "",
    // The target runtime to test for.
    string Runtime = "",
    // Directory in which to find the binaries to run. If not specified, the default path is ./bin/<configuration>/<framework>/. For projects with multiple target frameworks (via the TargetFrameworks property), you also need to define --framework when you specify this option. dotnet test always runs tests from the output directory. You can use AppDomain.BaseDirectory to consume test assets in the output directory.
    string Output = "",
    // Enables diagnostic mode for the test platform and writes diagnostic messages to the specified file and to files next to it. The process that is logging the messages determines which files are created, such as *.host_<date>.txt for test host log, and *.datacollector_<date>.txt for data collector log.
    string Diag = "",
    // Doesn't build the test project before running it. It also implicitly sets the - --no-restore flag.
    bool? NoBuild = default,
    // The directory where the test results are going to be placed. If the specified directory doesn't exist, it's created. The default is TestResults in the directory that contains the project file.
    string ResultsDirectory = "",
    // Enables data collector for the test run. For more information, see Monitor and analyze test run.
    string Collect = "",
    // Runs the tests in blame mode. This option is helpful in isolating problematic tests that cause the test host to crash. When a crash is detected, it creates a sequence file in TestResults/<Guid>/<Guid>_Sequence.xml that captures the order of tests that were run before the crash.
    bool? Blame = default,
    // Runs the tests in blame mode and collects a crash dump when the test host exits unexpectedly. This option depends on the version of .NET used, the type of error, and the operating system.
    bool? BlameCrash = default,
    // The type of crash dump to be collected. Implies BlameCrash.
    string BlameCrashDumpType = "",
    // Collects a crash dump on expected as well as unexpected test host exit.
    bool? BlameCrashCollectAlways = default,
    // Run the tests in blame mode and collects a hang dump when a test exceeds the given timeout.
    bool? BlameHang = default,
    // The type of crash dump to be collected. It should be full, mini, or none. When none is specified, test host is terminated on timeout, but no dump is collected. Implies BlameHang.
    string BlameHangDumpType = "",
    // Per-test timeout, after which a hang dump is triggered and the test host process and all of its child processes are dumped and terminated.
    TimeSpan? BlameHangTimeout = default,
    // Run tests without displaying the Microsoft TestPlatform banner. Available since .NET Core 3.0 SDK.
    bool? NoLogo = default,
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
    public DotNetTest(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host)
    {
        var blameHangTimeout = BlameHangTimeout?.TotalMilliseconds;
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<IDotNetSettings>();
        var cmd = host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("test")
            .AddNotEmptyArgs(Project)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildLoggers(host, Verbosity)
            .AddArgs(
                Loggers.Select(i => ("--logger", (string?)i))
                    .Concat(new[] {("--logger", (string?)"logger://teamcity")})
                    .Concat(new []{("--logger", (string?)"logger://teamcity")})
                    .ToArray())
            .AddArgs(
                ("--settings", Settings),
                ("--filter", Filter),
                ("--test-adapter-path", $"{string.Join(';', new[]{TestAdapterPath, virtualContext.Resolve(settings.DotNetVSTestLoggerDirectory)}.Where(i => !string.IsNullOrWhiteSpace(i)))}"),
                ("--configuration", Configuration),
                ("--framework", Framework),
                ("--runtime", Runtime),
                ("--output", Output),
                ("--diag", Diag),
                ("--results-directory", ResultsDirectory),
                ("--collect", Collect),
                ("--verbosity", Verbosity?.ToString().ToLowerInvariant()),
                ("--arch", Arch),
                ("--os", OS),
                ("--blame-crash-dump-type", BlameCrashDumpType),
                ("--blame-hang-dump-type", BlameHangDumpType),
                ("--blame-hang-timeout", blameHangTimeout.HasValue ? $"{(int)blameHangTimeout}milliseconds" : string.Empty)
            )
            .AddBooleanArgs(
                ("--list-tests", ListTests),
                ("--no-build", NoBuild),
                ("--blame", Blame),
                ("--blame-crash", BlameCrash),
                ("--blame-crash-collect-always", BlameCrashCollectAlways),
                ("--blame-hang", BlameHang),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore)
            )
            .AddProps("-p", Props.ToArray())
            .AddArgs(Args.ToArray());

        var runSettings = RunSettings.Select(i => $"{i.name}={i.value}").ToArray();
        if (runSettings.Any())
        {
            cmd = cmd.AddArgs("--").AddArgs(runSettings);
        }

        return cmd;
    }

    public override string ToString() => "dotnet test".GetShortName(ShortName, Project);
}