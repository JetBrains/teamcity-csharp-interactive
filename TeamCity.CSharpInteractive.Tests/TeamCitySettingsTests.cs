namespace TeamCity.CSharpInteractive.Tests
{
    using Moq;
    using Shouldly;
    using Xunit;

    public class TeamCitySettingsTests
    {
        private readonly Mock<IHostEnvironment> _hostEnvironment;
        private readonly Mock<IEnvironment> _environment;

        public TeamCitySettingsTests()
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
        public void ShouldProvideIsUnderTeamCity(string? projectName, string? version, bool expectedFlowId)
        {
            // Given
            var settings = CreateInstance();
            _hostEnvironment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME")).Returns(projectName);
            _hostEnvironment.Setup(i => i.GetEnvironmentVariable("TEAMCITY_VERSION")).Returns(version);

            // When
            var actualIsUnderTeamCity = settings.IsUnderTeamCity;

            // Then
            actualIsUnderTeamCity.ShouldBe(expectedFlowId);
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

        private TeamCitySettings CreateInstance() => new(_hostEnvironment.Object, _environment.Object);
    }
}