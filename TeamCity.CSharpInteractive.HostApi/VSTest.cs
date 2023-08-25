// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace HostApi;

using Cmd;
using DotNet;
using Immutype;
using JetBrains.TeamCity.ServiceMessages;

/// <summary>
/// The dotnet vstest command runs the VSTest.Console command-line application to run automated unit tests.
/// </summary>
[Target]
public partial record VSTest(
    IEnumerable<string> TestFileNames,
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies RunSettings passed to tests.
    IEnumerable<(string name, string value)> RunSettings,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Specifies a logger for test results. Use "console;verbosity=detailed". Specify the parameter multiple times to enable multiple loggers.
    IEnumerable<string> Loggers,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Run tests with names that match the provided values. Separate multiple values with commas.
    string Tests = "",
    // Run tests that match the given expression. <EXPRESSION> is of the format <property>Operator<value>[|&<EXPRESSION>], where Operator is one of =, !=, or ~. Operator ~ has 'contains' semantics and is applicable for string properties like DisplayName. Parentheses () are used to group subexpressions.
    string TestCaseFilter = "",
    // Target .NET Framework version used for test execution. Examples of valid values are .NETFramework,Version=v4.6 or .NETCoreApp,Version=v1.0. Other supported values are Framework40, Framework45, FrameworkCore10, and FrameworkUap10.
    string Framework = "",
    // Target platform architecture used for test execution. Valid values are x86, x64, and ARM.
    VSTestPlatform? Platform = default,
    // Settings to use when running tests.
    string Settings = "",
    // Lists all discovered tests from the given test container.
    bool? ListTests = default,
    // Run tests in parallel. By default, all available cores on the machine are available for use. Specify an explicit number of cores by setting the MaxCpuCount property under the RunConfiguration node in the runsettings file.
    bool? Parallel = default,
    // Use custom test adapters from a given path (if any) in the test run.
    string TestAdapterPath = "",
    // Runs the tests in blame mode. This option is helpful in isolating the problematic tests causing test host to crash. It creates an output file in the current directory as Sequence.xml that captures the order of tests execution before the crash.
    bool? Blame = default,
    // Enables verbose logs for the test platform. Logs are written to the provided file.
    string Diag = "",
    // Test results directory will be created in specified path if not exists.
    string ResultsDirectory = "",
    // Process ID of the parent process responsible for launching the current process.
    int? ParentProcessId = default,
    // Specifies the port for the socket connection and receiving the event messages.
    int? Port = default,
    string Collect = "",
    // Runs the tests in an isolated process. This makes vstest.console.exe process less likely to be stopped on an error in the tests, but tests may run slower.
    bool? InIsolation = default,
    // Sets the verbosity level of the command. Allowed values are Quiet, Minimal, Normal, Detailed, and Diagnostic. The default is Minimal. For more information, see LoggerVerbosity.
    DotNetVerbosity? Verbosity = default,
    // Specifies a short name for this operation.
    string ShortName = "")
{
    public VSTest(params string[] args)
        : this(Enumerable.Empty<string>(), args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host)
    {
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<IDotNetSettings>();
        var cmd = host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("vstest")
            .AddArgs(TestFileNames.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildArgs(
                ("--Logger", "logger://teamcity"),
                ("--Logger", $"console;verbosity={(Verbosity.HasValue ? (Verbosity.Value >= DotNetVerbosity.Normal ? Verbosity.Value : DotNetVerbosity.Normal) : DotNetVerbosity.Normal).ToString().ToLowerInvariant()}"),
                ("--TestAdapterPath", $"{string.Join(';', new[]{TestAdapterPath, virtualContext.Resolve(settings.DotNetVSTestLoggerDirectory)}.Where(i => !string.IsNullOrWhiteSpace(i)))}"),
                ("--Tests", Tests),
                ("--TestCaseFilter", TestCaseFilter),
                ("--Framework", Framework),
                ("--Platform", Platform?.ToString()),
                ("--Settings", Settings),
                ("--Diag", Diag),
                ("--ParentProcessId", ParentProcessId?.ToString()),
                ("--Port", Port?.ToString()),
                ("--Collect", Collect))
            .AddMSBuildArgs(Loggers.Select(i => ("--logger", (string?)i)).ToArray())
            .AddTeamCityEnvironmentVariables(host)
            .AddBooleanArgs(
                ("--ListTests", ListTests),
                ("--Parallel", Parallel),
                ("--Blame", Blame),
                ("--InIsolation", InIsolation)
            )
            .AddArgs(Args.ToArray());

        if (!string.IsNullOrWhiteSpace(ResultsDirectory))
        {
            cmd = cmd.AddArgs($"--ResultsDirectory:{ResultsDirectory}");
        }

        var runSettings = RunSettings.Select(i => $"{i.name}={i.value}").ToArray();
        if (runSettings.Any())
        {
            cmd = cmd.AddArgs("--").AddArgs(runSettings);
        }

        return cmd;
    }

    public void PreRun(IHost host) => host.GetService<IDotNetTestReportingService>().SendTestResultsStreamingDataMessageIfNeeded();

    public IEnumerable<IServiceMessage> GetNonStdStreamsServiceMessages(IHost host) =>
        host.GetService<IDotNetTestReportingService>().GetServiceMessagesFromFilesWithTestReports();

    public override string ToString() => (string.IsNullOrWhiteSpace(ShortName) ? "dotnet vstest" : ShortName).GetShortName(ShortName, string.Empty);
}
