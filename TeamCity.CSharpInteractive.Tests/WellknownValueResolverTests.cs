namespace TeamCity.CSharpInteractive.Tests;

using Cmd;
using Moq;
using Shouldly;
using Xunit;

public class WellknownValueResolverTests
{
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IDotnetEnvironment> _dotnetEnvironment = new();
    private readonly Mock<IDockerEnvironment> _dockerEnvironment = new();
    private readonly Mock<ITeamCitySettings> _teamCitySettings = new();

    [Fact]
    public void ShouldResolveDotnetExecutablePath()
    {
        // Given
        var resolver = CreateInstance();

        // When
        _dotnetEnvironment.SetupGet(i => i.Path).Returns("Bin");

        // Then
        resolver.Resolve(WellknownValue.DotnetExecutablePath).ShouldBe("Bin");
    }
    
    [Fact]
    public void ShouldResolveDotnetLoggerDirectory()
    {
        // Given
        var resolver = CreateInstance();

        // When
        _environment.Setup(i => i.GetPath(SpecialFolder.Bin)).Returns("Bin");

        // Then
        resolver.Resolve(WellknownValue.DotnetLoggerDirectory).ShouldBe("Bin");
    }
    
    [Fact]
    public void ShouldResolveTeamCityMessagesPath()
    {
        // Given
        var resolver = CreateInstance();

        // When
        _teamCitySettings.SetupGet(i => i.ServiceMessagesPath).Returns("Tmp");

        // Then
        resolver.Resolve(WellknownValue.TeamCityMessagesPath).ShouldBe("Tmp");
    }
    
    [Fact]
    public void ShouldResolveDockerExecutablePath()
    {
        // Given
        var resolver = CreateInstance();

        // When
        _dockerEnvironment.SetupGet(i => i.Path).Returns("Bin");

        // Then
        resolver.Resolve(WellknownValue.DockerExecutablePath).ShouldBe("Bin");
    }

    private WellknownValueResolver CreateInstance() =>
        new(_environment.Object, _dotnetEnvironment.Object, _dockerEnvironment.Object, _teamCitySettings.Object);
}