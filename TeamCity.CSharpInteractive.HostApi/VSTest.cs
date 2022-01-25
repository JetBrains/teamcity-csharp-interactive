// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;

[Immutype.Target]
public record VSTest(
    IEnumerable<string> TestFileNames,
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> RunSettings,
    IEnumerable<(string name, string value)> Vars,
    IEnumerable<string> Loggers,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Tests = "",
    string TestCaseFilter = "",
    string Framework = "",
    VSTestPlatform? Platform = default,
    string Settings = "",
    bool ListTests = false,
    bool Parallel = false,
    string TestAdapterPath = "",
    bool Blame = false,
    string Diag = "",
    string ResultsDirectory = "",
    int? ParentProcessId = default,
    int? Port = default,
    string Collect = "",
    bool InIsolation = false,
    DotNetVerbosity? Verbosity = default,
    string ShortName = "")
    : ICommandLine
{
    public VSTest(params string[] args)
        : this(Enumerable.Empty<string>(), args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>())
    { }
        
    public IStartInfo GetStartInfo(IHost host)
    {
        var cmd =  new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet vstest")
            .WithArgs("vstest")
            .AddArgs(TestFileNames.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddVSTestIntegration(host, Verbosity)
            .AddArgs(
                ("--Tests", Tests),
                ("--TestCaseFilter", TestCaseFilter),
                ("--Framework", Framework),
                ("--Platform", Platform?.ToString()),
                ("--Settings", Settings),
                ("--TestAdapterPath", TestAdapterPath),
                ("--Diag", Diag),
                ("--ResultsDirectory", ResultsDirectory),
                ("--ParentProcessId", ParentProcessId?.ToString()),
                ("--Port", Port?.ToString()),
                ("--Collect", Collect)
            )
            .AddBooleanArgs(
                ("--ListTests", ListTests),
                ("--Parallel", Parallel),
                ("--Blame", Blame),
                ("--InIsolation", InIsolation)
            )
            .AddArgs(Loggers.ToArray())
            .AddArgs(Args.ToArray());
                
        var runSettings = RunSettings.Select(i => $"{i.name}={i.value}").ToArray();
        if(runSettings.Any())
        {
            cmd = cmd.AddArgs("--").AddArgs(runSettings);
        }

        return cmd;
    }
}