// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace DotNet;

using Cmd;
using TeamCity.CSharpInteractive.Contracts;

[Immutype.Target]
public record Publish(
    IEnumerable<(string name, string value)> Props,
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Project = "",
    bool UseCurrentRuntime = false,
    string Output = "",
    string Manifest = "",
    bool NoBuild = false,
    bool SelfContained = false,
    bool NoSelfContained = false,
    bool NoLogo = false,
    string Framework = "",
    string Runtime = "",
    string Configuration = "",
    string VersionSuffix = "",
    bool NoRestore = false,
    string Arch = "",
    string OS = "",
    Verbosity? Verbosity = default,
    string ShortName = "")
    : IProcess
{
    public Publish(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }
        
    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<ISettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet pack")
            .WithArgs("publish")
            .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildIntegration(host, Verbosity)
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
                ("--self-contained", SelfContained),
                ("--no-self-contained", NoSelfContained),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore)
            )
            .AddProps("/p", Props.ToArray())
            .AddArgs(Args.ToArray());
}