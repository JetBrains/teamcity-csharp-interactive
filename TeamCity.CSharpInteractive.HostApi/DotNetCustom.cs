// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace HostApi;

using DotNet;
using Immutype;

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
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(string.IsNullOrWhiteSpace(ShortName) ? ((ExecutablePath == string.Empty ? "dotnet" : Path.GetFileNameWithoutExtension(ExecutablePath)) + " " + Args.FirstOrDefault()).TrimEnd() : ShortName)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .WithArgs(Args.ToArray());
}