namespace TeamCity.CSharpInteractive.Tests;

public class SettingTests
{
    [Fact]
    public void ShouldSetAndGetValue()
    {
        // Given
        var setting = CreateInstance();

        // When
        var prevValue = setting.SetSetting(WarningLevel.L5);

        // Then
        prevValue.ShouldBe(WarningLevel.L3);
        setting.GetSetting().ShouldBe(WarningLevel.L5);
    }

    private static Setting<WarningLevel> CreateInstance() =>
        new(WarningLevel.L3);
}