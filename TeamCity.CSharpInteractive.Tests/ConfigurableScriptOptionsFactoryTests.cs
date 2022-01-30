namespace TeamCity.CSharpInteractive.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;

public class ConfigurableScriptOptionsFactoryTests
{
    private readonly Mock<ISettingGetter<LanguageVersion>> _languageVersion = new();
    private readonly Mock<ISettingGetter<OptimizationLevel>> _optimizationLevel = new();
    private readonly Mock<ISettingGetter<WarningLevel>> _warningLevel = new();
    private readonly Mock<ISettingGetter<CheckOverflow>> _checkOverflow = new();
    private readonly Mock<ISettingGetter<AllowUnsafe>> _allowUnsafe = new();

    public ConfigurableScriptOptionsFactoryTests()
    {
        _languageVersion.Setup(i => i.GetSetting()).Returns(LanguageVersion.Default);
        _optimizationLevel.Setup(i => i.GetSetting()).Returns(OptimizationLevel.Debug);
        _warningLevel.Setup(i => i.GetSetting()).Returns(WarningLevel.L0);
        _checkOverflow.Setup(i => i.GetSetting()).Returns(CheckOverflow.On);
        _allowUnsafe.Setup(i => i.GetSetting()).Returns(AllowUnsafe.Off);
    }

    [Theory]
    [InlineData(OptimizationLevel.Debug)]
    [InlineData(OptimizationLevel.Release)]
    public void ShouldConfigureOptimizationLevel(OptimizationLevel value)
    {
        // Given
        var factory = CreateInstance();
        _optimizationLevel.Setup(i => i.GetSetting()).Returns(value);

        // When
        var options = factory.Create(ScriptOptions.Default);

        // Then
        options.OptimizationLevel.ShouldBe(value);
    }

    [Theory]
    [InlineData(WarningLevel.L0, 0)]
    [InlineData(WarningLevel.L1, 1)]
    [InlineData(WarningLevel.L2, 2)]
    [InlineData(WarningLevel.L3, 3)]
    [InlineData(WarningLevel.L4, 4)]
    [InlineData(WarningLevel.L5, 5)]
    internal void ShouldConfigureWarningLevel(WarningLevel value, int expectedValue)
    {
        // Given
        var factory = CreateInstance();
        _warningLevel.Setup(i => i.GetSetting()).Returns(value);

        // When
        var options = factory.Create(ScriptOptions.Default);

        // Then
        options.WarningLevel.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData(CheckOverflow.Off, false)]
    [InlineData(CheckOverflow.On, true)]
    internal void ShouldConfigureCheckOverflow(CheckOverflow value, bool expectedValue)
    {
        // Given
        var factory = CreateInstance();
        _checkOverflow.Setup(i => i.GetSetting()).Returns(value);

        // When
        var options = factory.Create(ScriptOptions.Default);

        // Then
        options.CheckOverflow.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData(AllowUnsafe.Off, false)]
    [InlineData(AllowUnsafe.On, true)]
    internal void ShouldConfigureAllowUnsafe(AllowUnsafe value, bool expectedValue)
    {
        // Given
        var factory = CreateInstance();
        _allowUnsafe.Setup(i => i.GetSetting()).Returns(value);

        // When
        var options = factory.Create(ScriptOptions.Default);

        // Then
        options.AllowUnsafe.ShouldBe(expectedValue);
    }

    private ConfigurableScriptOptionsFactory CreateInstance() =>
        new(_languageVersion.Object, _optimizationLevel.Object, _warningLevel.Object, _checkOverflow.Object, _allowUnsafe.Object);
}