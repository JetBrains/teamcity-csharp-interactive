namespace TeamCity.CSharpInteractive.Tests
{
    using Contracts;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Moq;
    using Xunit;

    public class TeamCityPropertiesTests
    {
        private readonly Mock<IProperties> _properties;
        private readonly Mock<ITeamCityBuildStatusWriter> _teamCityBuildStatusWriter;

        public TeamCityPropertiesTests()
        {
            _properties = new Mock<IProperties>();
            _teamCityBuildStatusWriter = new Mock<ITeamCityBuildStatusWriter>();
        }

        [Fact]
        public void ShouldSetProperty()
        {
            // Given
            var props = CreateInstance();

            // When
            props["Abc"] = "Xyz";

            // Then
            _properties.VerifySet(i => i["Abc"] = "Xyz");
            _teamCityBuildStatusWriter.Verify(i => i.WriteBuildParameter("system.Abc", "Xyz"));
        }

        private TeamCityProperties CreateInstance() =>
            new(_properties.Object, _teamCityBuildStatusWriter.Object);
    }
}