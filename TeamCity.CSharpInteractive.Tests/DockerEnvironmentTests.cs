namespace TeamCity.CSharpInteractive.Tests;

using Microsoft.DotNet.PlatformAbstractions;

public class DockerEnvironmentTests
{
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IFileExplorer> _fileExplorer = new();

    [Theory]
    [InlineData(Platform.Windows, "docker.exe")]
    [InlineData(Platform.Linux, "docker")]
    [InlineData(Platform.FreeBSD, "docker")]
    [InlineData(Platform.Darwin, "docker")]
    [InlineData(Platform.Unknown, "docker")]
    public void ShouldFindPath(Platform platform, string defaultPath)
    {
        // Given
        _environment.SetupGet(i => i.OperatingSystemPlatform).Returns(platform);
        _fileExplorer.Setup(i => i.FindFiles(defaultPath, "DOCKER_HOME")).Returns(new [] { "Abc", "Xyz" });

        // When
        var instance = CreateInstance();

        // Then
        instance.Path.ShouldBe("Abc");
    }
        
    [Theory]
    [InlineData(Platform.Windows, "docker.exe")]
    [InlineData(Platform.Linux, "docker")]
    [InlineData(Platform.FreeBSD, "docker")]
    [InlineData(Platform.Darwin, "docker")]
    [InlineData(Platform.Unknown, "docker")]
    public void ShouldProvideDefaultPathWhenException(Platform platform, string defaultPath)
    {
        // Given
        _environment.SetupGet(i => i.OperatingSystemPlatform).Returns(platform);
        _fileExplorer.Setup(i => i.FindFiles(defaultPath, "DOCKER_HOME")).Throws<Exception>();

        // When
        var instance = CreateInstance();

        // Then
        instance.Path.ShouldBe(defaultPath);
    }

    private DockerEnvironment CreateInstance() =>
        new(_environment.Object, _fileExplorer.Object);
}