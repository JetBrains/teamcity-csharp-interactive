namespace TeamCity.CSharpInteractive.Tests;

public class TeamCitySpecificTests
{
    private readonly Mock<ITeamCitySettings> _settings = new();

    [Theory]
    [InlineData(true, 2)]
    [InlineData(false, 1)]
    public void ShouldProvideValueDependingItIsUnderTeamCity(bool isUnderTeamCity, int expectedResult)
    {
        // Given
        var instance = CreateInstance();

        // When
        _settings.SetupGet(i => i.IsUnderTeamCity).Returns(isUnderTeamCity);

        // Then
        instance.Instance.ShouldBe(expectedResult);
    }

    private TeamCitySpecific<int> CreateInstance() =>
        new(_settings.Object, () => 1, () => 2);
}