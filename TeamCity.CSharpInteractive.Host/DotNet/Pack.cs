// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace DotNet;

using Cmd;
using Script;

[Immutype.Target]
public record Pack(
    IEnumerable<(string name, string value)> Props,
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Project = "",
    string Output = "",
    bool NoBuild = false,
    bool IncludeSymbols = false,
    bool IncludeSource = false,
    bool Serviceable = false,
    bool NoLogo = false,
    bool NoRestore = false,
    string VersionSuffix = "",
    string Configuration = "",
    bool UseCurrentRuntime = false,
    Verbosity? Verbosity = default,
    string ShortName = "")
    : IProcess
{
    public Pack(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }
        
    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<ISettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet pack")
            .WithArgs("pack")
            .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildIntegration(host, Verbosity)
            .AddArgs(
                ("--output", Output),
                ("--version-suffix", VersionSuffix),
                ("--configuration", Configuration)
            )
            .AddBooleanArgs(
                ("--no-build", NoBuild),
                ("--include-symbols", IncludeSymbols),
                ("--include-source", IncludeSource),
                ("--serviceable", Serviceable),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore),
                ("--use-current-runtime", UseCurrentRuntime)
            )
            .AddProps("/p", Props.ToArray())
            .AddArgs(Args.ToArray());
}