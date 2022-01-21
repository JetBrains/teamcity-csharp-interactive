namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using Script;

[ExcludeFromCodeCoverage]
internal readonly record struct ScriptHostComponents(
    IHost Host,
    IStatistics Statistics,
    IPresenter<Summary> SummaryPresenter,
    ILog<ScriptHostComponents> Log,
    IInfo Info,
    ISettingsManager SettingsManager,
    IExitTracker ExitTracker);