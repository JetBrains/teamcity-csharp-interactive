// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class DockerSettings : Docker.ISettings
{
    private readonly IDockerEnvironment _dockerEnvironment;

    public DockerSettings(IDockerEnvironment dockerEnvironment) =>
        _dockerEnvironment = dockerEnvironment;

    public string DockerExecutablePath => _dockerEnvironment.Path;
}