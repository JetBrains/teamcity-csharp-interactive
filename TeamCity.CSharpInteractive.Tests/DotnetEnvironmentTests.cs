namespace TeamCity.CSharpInteractive.Tests;

using Microsoft.DotNet.PlatformAbstractions;

public class DotNetEnvironmentTests
{
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IFileExplorer> _fileExplorer = new();

    [Fact]
    public void ShouldProvideTargetFrameworkMoniker()
    {
        // Given

        // When
        var instance = CreateInstance(".NETCoreApp,Version=v3.1", "dotnet.exe");

        // Then
        instance.TargetFrameworkMoniker.ShouldBe(".NETCoreApp,Version=v3.1");
    }

    [Theory]
    [InlineData(Platform.Windows, "dotnet.exe")]
    [InlineData(Platform.Linux, "dotnet")]
    [InlineData(Platform.FreeBSD, "dotnet")]
    [InlineData(Platform.Darwin, "dotnet")]
    [InlineData(Platform.Unknown, "dotnet")]
    public void ShouldProvidePathFromModule(Platform platform, string dotnetExecutable)
    {
        // Given
        _environment.SetupGet(i => i.OperatingSystemPlatform).Returns(platform);
        var modulePath = Path.Combine("Bin", dotnetExecutable);

        // When
        var instance = CreateInstance(".NETCoreApp,Version=v3.1", modulePath);

        // Then
        instance.Path.ShouldBe(modulePath);
    }

    [Theory]
    [InlineData(Platform.Windows, "dotnet.exe")]
    [InlineData(Platform.Linux, "dotnet")]
    [InlineData(Platform.FreeBSD, "dotnet")]
    [InlineData(Platform.Darwin, "dotnet")]
    [InlineData(Platform.Unknown, "dotnet")]
    public void ShouldFindPath(Platform platform, string defaultPath)
    {
        // Given
        _environment.SetupGet(i => i.OperatingSystemPlatform).Returns(platform);
        _fileExplorer.Setup(i => i.FindFiles(defaultPath, "DOTNET_ROOT", "DOTNET_HOME")).Returns(new[] {"Abc", "Xyz"});

        // When
        var instance = CreateInstance(".NETCoreApp,Version=v3.1", "Abc");

        // Then
        instance.Path.ShouldBe("Abc");
    }

    [Theory]
    [InlineData(Platform.Windows, "dotnet.exe")]
    [InlineData(Platform.Linux, "dotnet")]
    [InlineData(Platform.FreeBSD, "dotnet")]
    [InlineData(Platform.Darwin, "dotnet")]
    [InlineData(Platform.Unknown, "dotnet")]
    public void ShouldProvideDefaultPathWhenException(Platform platform, string defaultPath)
    {
        // Given
        _environment.SetupGet(i => i.OperatingSystemPlatform).Returns(platform);
        _fileExplorer.Setup(i => i.FindFiles(defaultPath, "DOTNET_ROOT", "DOTNET_HOME")).Throws<Exception>();

        // When
        var instance = CreateInstance(".NETCoreApp,Version=v3.1", "Abc");

        // Then
        instance.Path.ShouldBe(defaultPath);
    }

    private DotNetEnvironment CreateInstance(string frameworkName, string moduleFile) =>
        new(frameworkName, moduleFile, _environment.Object, _fileExplorer.Object);
}