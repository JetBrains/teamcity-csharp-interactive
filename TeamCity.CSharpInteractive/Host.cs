// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Loader;
using Contracts;

[ExcludeFromCodeCoverage]
public static class Host
{
    private static readonly HostComponents Components = Composer.ResolveHostComponents();
    private static readonly IDisposable StatisticsToken;

    static Host()
    {
        StatisticsToken = Components.Statistics.Start();
        AssemblyLoadContext.Default.Unloading += OnDefaultOnUnloading;
    }

    public static IReadOnlyList<string> Args => Components.Host.Args;

    public static IProperties Props => Components.Host.Props;

    public static void WriteLine() => Components.Host.WriteLine();

    public static void WriteLine<T>(T line, Color color = Color.Default) => Components.Host.WriteLine(line, color);

    public static void Error(string? error, string? errorId = default) => Components.Host.Error(error, errorId);

    public static void Warning(string? warning) => Components.Host.Warning(warning);

    public static void Info(string? text) => Components.Host.Info(text);

    public static void Trace(string? trace, string? origin = default) => Components.Host.Trace(trace, origin);

    public static T GetService<T>() => Components.Host.GetService<T>();

    public static void Exit(int exitCode = 0) => System.Environment.Exit(exitCode);

    private static void OnDefaultOnUnloading(AssemblyLoadContext ctx)
    {
        try
        {
            StatisticsToken.Dispose();
            Components.StatisticsPresenter.Show(Components.Statistics);
            var state = Components.Statistics.Errors.Any()
                ? new Text("Running FAILED.", Color.Error)
                : new Text("Running succeeded.", Color.Success);
            Components.Log.Info(state);
        }
        catch (Exception ex)
        {
            try
            {
                Components.Log.Error(ErrorId.Unhandled, ex.Message);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}