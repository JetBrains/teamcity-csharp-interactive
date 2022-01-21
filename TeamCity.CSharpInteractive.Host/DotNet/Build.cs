// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace Script.DotNet;

using Cmd;
using Script;

[Immutype.Target]
public record Build(
    IEnumerable<(string name, string value)> Props,
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Project = "",
    string Output = "",
    string Framework = "",
    string Configuration = "",
    string Runtime = "",
    string VersionSuffix = "",
    bool NoIncremental = false,
    bool NoDependencies = false,
    bool NoLogo = false,
    bool NoRestore = false,
    bool Force = false,
    Verbosity? Verbosity = default,
    string ShortName = "")
    : ICommandLine
{
    public Build(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<ISettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet build")
            .WithArgs("build")
            .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddMSBuildIntegration(host, Verbosity)
            .AddArgs(
                ("--output", Output),
                ("--framework", Framework),
                ("--configuration", Configuration),
                ("--runtime", Runtime),
                ("--version-suffix", VersionSuffix),
                ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--no-incremental", NoIncremental),
                ("--no-dependencies", NoDependencies),
                ("--nologo", NoLogo),
                ("--no-restore", NoRestore),
                ("--force", Force)
            )
            .AddProps("/p", Props.ToArray())
            .AddArgs(Args.ToArray());
}