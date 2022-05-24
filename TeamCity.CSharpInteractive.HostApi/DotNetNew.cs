// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace HostApi;

using DotNet;
using Immutype;

/// <summary>
/// The 'dotnet new' command creates a .NET project based on a template. 
/// </summary>
[Target]
public record DotNetNew(
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Specifies a short template name, for example 'console'.
    string TemplateName,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Specifies a short name for this operation.
    string ShortName = "")
    : ICommandLine
{
    public DotNetNew(string templateName, params string[] args)
        : this(args, Enumerable.Empty<(string, string)>(), templateName)
    { }
    
    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
            .WithShortName(ToString())
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .WithArgs("new", TemplateName)
            .AddArgs(Args.ToArray());

    public override string ToString() => (ExecutablePath == string.Empty ? "dotnet new" : Path.GetFileNameWithoutExtension(ExecutablePath)).GetShortName(ShortName, TemplateName);
}