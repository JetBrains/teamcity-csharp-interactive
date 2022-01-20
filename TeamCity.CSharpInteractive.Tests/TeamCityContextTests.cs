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
    public void ShouldGetDotNetLoggerDirectory()
    {
        // Given
        var settings = CreateInstance();

        // When
        _environment.Setup(i => i.GetPath(SpecialFolder.Bin)).Returns("Bin");

        // Then
        settings.DotNetLoggerDirectory.ShouldBe("Bin");
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