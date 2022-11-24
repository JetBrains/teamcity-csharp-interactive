// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests;

public class CISettingsTests
{
    private readonly Mock<IHostEnvironment> _hostEnvironment;
    private readonly Mock<IEnvironment> _environment;

    public CISettingsTests()
    {
        _hostEnvironment = new Mock<IHostEnvironment>();
        _environment = new Mock<IEnvironment>();
    }

    [Theory]
    [InlineData("Abc", "1", true)]
    [InlineData("Abc", null, true)]
    [InlineData("Abc", "  ", true)]
    [InlineData("Abc", "", true)]
    [InlineData(null, "1", true)]
    [InlineData("", "", false)]
    [InlineData("", "  ", false)]
    [InlineData("   ", "", false)]
    [InlineData("  ", "   ", false)]
    [InlineData("  ", null, false)]
    [InlineData(null, "  ", false)]
    [InlineData(null, null, false)]
    public void ShouldProvideIsUnderTeamCity(string? projectName, string? version, bool expectedIsUnderTeamCity)
    {
        // Given
        var settings = CreateInstance();
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME")).Returns(projectName);
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_VERSION")).Returns(version);

        // When
        var actualIsUnderTeamCity = settings.CIType;

        // Then
        actualIsUnderTeamCity.ShouldBe(expectedIsUnderTeamCity ? CIType.TeamCity: CIType.Unknown);
    }

    [Theory]
    [InlineData("Abc", "Abc")]
    [InlineData(null, "ROOT")]
    [InlineData("  ", "ROOT")]
    public void ShouldProvideFlowId(string? flowId, string expectedFlowId)
    {
        // Given
        var settings = CreateInstance();
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID")).Returns(flowId);

        // When
        var actualFlowId = settings.FlowId;

        // Then
        actualFlowId.ShouldBe(expectedFlowId);
    }

    private CISettings CreateInstance() => new(_hostEnvironment.Object, _environment.Object);
}