// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi.Docker;

internal class DockerSettings : IDockerSettings
{
    private readonly IDockerEnvironment _dockerEnvironment;

    public DockerSettings(IDockerEnvironment dockerEnvironment) =>
        _dockerEnvironment = dockerEnvironment;

    public string DockerExecutablePath => _dockerEnvironment.Path;
}