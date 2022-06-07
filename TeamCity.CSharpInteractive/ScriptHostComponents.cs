namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using HostApi;

[ExcludeFromCodeCoverage]
internal readonly record struct ScriptHostComponents(
    IHost Host,
    IStatistics Statistics,
    ILog<ScriptHostComponents> Log,
    IInfo Info,
    IExitTracker ExitTracker);