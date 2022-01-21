namespace TeamCity.CSharpInteractive.Tests;

using JetBrains.TeamCity.ServiceMessages.Write.Special;
using Script;

public class TeamCityPropertiesTests
{
    private readonly Mock<IProperties> _properties;
    private readonly Mock<ITeamCityWriter> _teamCityWriter;

    public TeamCityPropertiesTests()
    {
        _properties = new Mock<IProperties>();
        _teamCityWriter = new Mock<ITeamCityWriter>();
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
        _teamCityWriter.Verify(i => i.WriteBuildParameter("system.Abc", "Xyz"));
    }
        
    [Fact]
    public void ShouldGetProperty()
    {
        // Given
        var props = CreateInstance();
        var curVal = "Xyz";
        _properties.Setup(i => i.TryGetValue("Abc", out curVal)).Returns(true);

        // When
        props.TryGetValue("Abc", out var actual).ShouldBeTrue();

        // Then
        actual.ShouldBe("Xyz");
    }
        
    [Fact]
    public void ShouldProvideCount()
    {
        // Given
        var props = CreateInstance();
        _properties.SetupGet(i => i.Count).Returns(2);

        // When
        var actual = props.Count;

        // Then
        actual.ShouldBe(2);
    }

    private TeamCityProperties CreateInstance() =>
        new(_properties.Object, _teamCityWriter.Object);
}