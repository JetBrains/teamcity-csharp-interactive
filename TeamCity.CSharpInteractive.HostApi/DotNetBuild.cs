// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;

[Immutype.Target]
public record DotNetBuild(
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
    DotNetVerbosity? Verbosity = default,
    string ShortName = "")
    : ICommandLine
{
    public DotNetBuild(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName("dotnet build".GetShortName(ShortName, Project))
            .WithArgs("build")
            .AddNotEmptyArgs(Project)
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