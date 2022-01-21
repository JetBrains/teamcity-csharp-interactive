// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Loader;
using Script;

[ExcludeFromCodeCoverage]
public static class ScriptHost
{
    private static readonly ScriptHostComponents Components = Composer.ResolveScriptHostComponents();
    private static readonly IDisposable FinishToken;

    static ScriptHost()
    {
        Components.SettingsManager.Load();
        Components.Info.ShowHeader();
        FinishToken = Disposable.Create(Components.ExitTracker.Track(), Components.Statistics.Start());
        AssemblyLoadContext.Default.Unloading += OnDefaultOnUnloading;
    }

    public static IHost Host => Components.Host;

    public static IReadOnlyList<string> Args => Components.Host.Args;

    public static IProperties Props => Components.Host.Props;

    public static void WriteLine() => Components.Host.WriteLine();

    public static void WriteLine<T>(T line, Color color = Color.Default) => Components.Host.WriteLine(line, color);

    public static void Error(string? error, string? errorId = default) => Components.Host.Error(error, errorId);

    public static void Warning(string? warning) => Components.Host.Warning(warning);

    public static void Info(string? text) => Components.Host.Info(text);

    public static void Trace(string? trace, string? origin = default) => Components.Host.Trace(trace, origin);

    public static T GetService<T>() => Components.Host.GetService<T>();

    private static void OnDefaultOnUnloading(AssemblyLoadContext ctx)
    {
        try
        {
            FinishToken.Dispose();
            Components.SummaryPresenter.Show(Summary.Empty);
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