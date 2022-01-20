// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace DotNet;

using Cmd;
using Host;

[Immutype.Target]
public record Test(
    IEnumerable<(string name, string value)> Props,
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Project = "",
    string Settings = "",
    bool ListTests = false,
    string Filter = "",
    string TestAdapterPath = "",
    string Logger = "",
    string Configuration = "",
    string Framework = "",
    string Runtime = "",
    string Output = "",
    string Diag = "",
    bool NoBuild = false,
    string ResultsDirectory = "",
    string Collect = "",
    bool Blame = false,
    bool NoLogo = false,
    bool NoRestore = false,
    Verbosity? Verbosity = default,
    string ShortName = "")
    : IProcess
{
    public Test(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }
        
    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<ISettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet test")
            .WithArgs("test")
            .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildIntegration(host, Verbosity)
            .AddArgs(
                ("--settings", Settings),
                ("--filter", Filter),
                ("--test-adapter-path", TestAdapterPath),
                ("--logger", Logger),
                ("--configuration", Configuration),
                ("--framework", Framework),
                ("--runtime", Runtime),
                ("--output", Output),
                ("--diag", Diag),
                ("--results-directory", ResultsDirectory),
                ("--collect", Collect),
                ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--list-tests", ListTests),
                ("--no-build", NoBuild),
                ("--blame", Blame),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore)
            )
            .AddProps("/p", Props.ToArray())
            .AddArgs(Args.ToArray());
}