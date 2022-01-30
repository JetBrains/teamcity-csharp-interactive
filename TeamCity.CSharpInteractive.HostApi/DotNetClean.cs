// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

[Target]
public record DotNetClean(
    IEnumerable<(string name, string value)> Props,
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string Project = "",
    string Framework = "",
    string Runtime = "",
    string Configuration = "",
    string Output = "",
    bool NoLogo = false,
    DotNetVerbosity? Verbosity = default,
    string ShortName = "")
    : ICommandLine
{
    public DotNetClean(params string[] args)
        : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName("dotnet clean".GetShortName(ShortName, Project))
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
}