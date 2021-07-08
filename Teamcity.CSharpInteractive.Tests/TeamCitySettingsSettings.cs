namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Shouldly;
    using Xunit;

    public class TeamCitySettingsSettings
    {
        private readonly Mock<IHostEnvironment> _environment;

        public TeamCitySettingsSettings() => _environment = new Mock<IHostEnvironment>();

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
        public void ShouldProvideIsUnderTeamCity(string? projectName, string? version, bool expectedFlowId)
        {
            // Given
            var settings = CreateInstance();
            _environment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME")).Returns(projectName);
            _environment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_VERSION")).Returns(version);

            // When
            var actualIsUnderTeamCity = settings.IsUnderTeamCity;

            // Then
            actualIsUnderTeamCity.ShouldBe(expectedFlowId);
        }
        
        [Theory]
        [InlineData("Abc", "Abc")]
        [InlineData(null, "")]
        [InlineData("  ", "")]
        public void ShouldProvideFlowId(string? flowId, string expectedFlowId)
        {
            // Given
            var settings = CreateInstance();
            _environment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID")).Returns(flowId);

            // When
            var actualFlowId = settings.FlowId;

            // Then
            actualFlowId.ShouldBe(expectedFlowId);
        }

        private TeamCitySettings CreateInstance() => new(_environment.Object);
    }
}