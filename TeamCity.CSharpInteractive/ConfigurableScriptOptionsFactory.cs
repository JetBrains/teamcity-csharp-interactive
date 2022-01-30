// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

internal class ConfigurableScriptOptionsFactory : IScriptOptionsFactory
{
    private readonly ISettingGetter<LanguageVersion> _languageVersion;
    private readonly ISettingGetter<OptimizationLevel> _optimizationLevel;
    private readonly ISettingGetter<WarningLevel> _warningLevel;
    private readonly ISettingGetter<CheckOverflow> _checkOverflow;
    private readonly ISettingGetter<AllowUnsafe> _allowUnsafe;

    public ConfigurableScriptOptionsFactory(
        ISettingGetter<LanguageVersion> languageVersion,
        ISettingGetter<OptimizationLevel> optimizationLevel,
        ISettingGetter<WarningLevel> warningLevel,
        ISettingGetter<CheckOverflow> checkOverflow,
        ISettingGetter<AllowUnsafe> allowUnsafe)
    {
        _languageVersion = languageVersion;
        _optimizationLevel = optimizationLevel;
        _warningLevel = warningLevel;
        _checkOverflow = checkOverflow;
        _allowUnsafe = allowUnsafe;
    }

    public ScriptOptions Create(ScriptOptions baseOptions) =>
        baseOptions
            .WithLanguageVersion(_languageVersion.GetSetting())
            .WithOptimizationLevel(_optimizationLevel.GetSetting())
            .WithWarningLevel((int)_warningLevel.GetSetting())
            .WithCheckOverflow(_checkOverflow.GetSetting() == CheckOverflow.On)
            .WithAllowUnsafe(_allowUnsafe.GetSetting() == AllowUnsafe.On);
}