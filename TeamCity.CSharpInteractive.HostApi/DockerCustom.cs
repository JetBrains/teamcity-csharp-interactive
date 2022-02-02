// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace HostApi;

using Docker;
using Immutype;

[Target]
public record DockerCustom(
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    string ExecutablePath = "",
    string WorkingDirectory = "",
    string ShortName = "")
    : ICommandLine
{
    public DockerCustom(params string[] args)
        : this(args, Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDockerSettings>().DockerExecutablePath : ExecutablePath)
            .WithShortName(ToString())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .WithArgs(Args.ToArray());

    public override string ToString() => string.IsNullOrWhiteSpace(ShortName) ? ((ExecutablePath == string.Empty ? "docker" : Path.GetFileNameWithoutExtension(ExecutablePath)) + " " + Args.FirstOrDefault()).TrimEnd() : ShortName;
}