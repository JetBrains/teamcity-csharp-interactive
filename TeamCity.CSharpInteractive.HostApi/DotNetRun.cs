// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;

[Immutype.Target]
public record DotNetRun(
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Framework = "",
    string Configuration = "",
    string Runtime = "",
    string Project = "",
    string LaunchProfile = "",
    bool NoLaunchProfile = false,
    bool NoBuild = false,
    bool NoRestore = false,
    bool NoDependencies = false,
    bool Force = false,
    DotNetVerbosity? Verbosity = default,
    string ShortName = "")
    : ICommandLine
{
    public DotNetRun(params string[] args)
        : this(args, Enumerable.Empty<(string, string)>())
    { }
        
    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet run")
            .WithArgs("run")
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddArgs(
                ("--framework", Framework),
                ("--configuration", Configuration),
                ("--runtime", Runtime),
                ("--project", Project),
                ("--launch-profile", LaunchProfile),
                ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--no-launch-profile", NoLaunchProfile),
                ("--no-build", NoBuild),
                ("--no-restore", NoRestore),
                ("--no-dependencies", NoDependencies),
                ("--force", Force)
            )
            .AddArgs(Args.ToArray());
}