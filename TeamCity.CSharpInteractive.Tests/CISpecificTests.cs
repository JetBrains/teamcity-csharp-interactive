// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests;

public class CISpecificTests
{
    private readonly Mock<ICISettings> _settings = new();

    [Theory]
    [InlineData(true, 2)]
    [InlineData(false, 1)]
    public void ShouldProvideValueDependingItIsUnderTeamCity(bool isUnderTeamCity, int expectedResult)
    {
        // Given
        var instance = CreateInstance();

        // When
        _settings.SetupGet(i => i.CIType).Returns(isUnderTeamCity ? CIType.TeamCity : CIType.Unknown);

        // Then
        instance.Instance.ShouldBe(expectedResult);
    }

    private CISpecific<int> CreateInstance() =>
        new(_settings.Object, () => 1, () => 2, () => 3);
}