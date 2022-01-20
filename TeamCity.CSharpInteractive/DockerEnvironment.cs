// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using Microsoft.DotNet.PlatformAbstractions;

internal class DockerEnvironment : ITraceSource, IDockerEnvironment
{
    private readonly IEnvironment _environment;
    private readonly IFileExplorer _fileExplorer;

    public DockerEnvironment(
        IEnvironment environment,
        IFileExplorer fileExplorer)
    {
        _environment = environment;
        _fileExplorer = fileExplorer;
    }

    public string Path
    {
        get
        {
            var executable = _environment.OperatingSystemPlatform == Platform.Windows ? "docker.exe" : "docker";
            try
            {
                return _fileExplorer.FindFiles(executable, "DOCKER_HOME").FirstOrDefault() ?? executable;
            }
            catch
            {
                // ignored
            }
                
            return executable;
        }
    }

    [ExcludeFromCodeCoverage]
    public IEnumerable<Text> Trace
    {
        get
        {
            yield return new Text($"DockerPath: {Path}");
        }
    }
}