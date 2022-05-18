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
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
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