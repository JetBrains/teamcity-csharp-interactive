// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests;

public class TeamCityContextTests
{
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IDotNetEnvironment> _dotnetEnvironment = new();
    private readonly Mock<ITeamCitySettings> _teamCitySettings = new();

    [Fact]
    public void ShouldGetDotNetExecutablePath()
    {
        // Given
        var settings = CreateInstance();

        // When
        _dotnetEnvironment.SetupGet(i => i.Path).Returns("Bin");

        // Then
        settings.DotNetExecutablePath.ShouldBe("Bin");
    }

    [Fact]
    public void ShouldGetDotNetMSBuildLoggerDirectory()
    {
        // Given
        var settings = CreateInstance();

        // When
        _environment.Setup(i => i.GetPath(SpecialFolder.Bin)).Returns("Bin");

        // Then
        settings.DotNetMSBuildLoggerDirectory.ShouldBe(Path.Combine("Bin", "msbuild"));
    }

    [Fact]
    public void ShouldGetDotNetVSTestLoggerDirectory()
    {
        // Given
        var settings = CreateInstance();

        // When
        _environment.Setup(i => i.GetPath(SpecialFolder.Bin)).Returns("Bin");

        // Then
        settings.DotNetVSTestLoggerDirectory.ShouldBe(Path.Combine("Bin", "vstest"));
    }

    [Fact]
    public void ShouldGetTeamCityServiceMessagesBackupPathEnvValue()
    {
        // Given
        var resolver = CreateInstance();

        // When
        _teamCitySettings.SetupGet(i => i.ServiceMessagesBackupPathEnvValue).Returns("Tmp");

        // Then
        resolver.TeamCityServiceMessagesBackupPathEnvValue.ShouldBe("Tmp");
    }

    private TeamCityContext CreateInstance() =>
        new(_environment.Object, _dotnetEnvironment.Object, _teamCitySettings.Object);
}
