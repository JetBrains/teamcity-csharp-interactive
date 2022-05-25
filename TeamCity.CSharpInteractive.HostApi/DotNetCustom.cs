// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace HostApi;

using DotNet;
using Immutype;

/// <summary>
/// The dotnet custom command is used to execute any dotnet commands with any arguments. 
/// </summary>
[Target]
public record DotNetCustom(
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Specifies whether the ability to use build loggers is supported. The default is set to true.
    string ShortName = "")
    : ICommandLine
{
    public DotNetCustom(params string[] args)
        : this(args, Enumerable.Empty<(string, string)>())
    { }
    
    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .WithArgs(Args.ToArray());

    public override string ToString() => (ExecutablePath == string.Empty ? "dotnet" : Path.GetFileNameWithoutExtension(ExecutablePath)).GetShortName(ShortName, Args.FirstOrDefault() ?? string.Empty);
}