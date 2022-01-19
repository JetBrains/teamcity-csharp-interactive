namespace TeamCity.CSharpInteractive.Tests;

using Moq;
using Shouldly;
using Xunit;

public class TeamCityContextTests
{
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IDotnetEnvironment> _dotnetEnvironment = new();
    private readonly Mock<ITeamCitySettings> _teamCitySettings = new();
    
    [Fact]
    public void ShouldGetDotnetExecutablePath()
    {
        // Given
        var settings = CreateInstance();

        // When
        _dotnetEnvironment.SetupGet(i => i.Path).Returns("Bin");

        // Then
        settings.DotnetExecutablePath.ShouldBe("Bin");
    }
    
    [Fact]
    public void ShouldGetDotnetLoggerDirectory()
    {
        // Given
        var settings = CreateInstance();

        // When
        _environment.Setup(i => i.GetPath(SpecialFolder.Bin)).Returns("Bin");

        // Then
        settings.DotnetLoggerDirectory.ShouldBe("Bin");
    }
    
    [Fact]
    public void ShouldGetTeamCityMessagesPath()
    {
        // Given
        var resolver = CreateInstance();

        // When
        _teamCitySettings.SetupGet(i => i.ServiceMessagesPath).Returns("Tmp");

        // Then
        resolver.TeamCityMessagesPath.ShouldBe("Tmp");
    }

    private TeamCityContext CreateInstance() =>
        new(_environment.Object, _dotnetEnvironment.Object, _teamCitySettings.Object);
}