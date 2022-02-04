// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
using System.Diagnostics.CodeAnalysis;
// ReSharper disable once RedundantUsingDirective
using System.Runtime.Loader;
using HostApi;
using TeamCity.CSharpInteractive;

[ExcludeFromCodeCoverage]
public static class Host
{
    private static readonly ScriptHostComponents Components = Composer.ResolveScriptHostComponents();
    private static readonly IHost CurHost = Components.Host;

#if APPLICATION
    private static readonly IDisposable FinishToken;

    static Host()
    {
        Components.Info.ShowHeader();
        FinishToken = Disposable.Create(Components.ExitTracker.Track(), Components.Statistics.Start());
        AssemblyLoadContext.Default.Unloading += OnDefaultOnUnloading;
    }
    
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
#endif

    public static IReadOnlyList<string> Args => CurHost.Args;

    public static IProperties Props => CurHost.Props;

    public static void WriteLine() => CurHost.WriteLine();

    public static void WriteLine<T>(T line, Color color = Color.Default) => CurHost.WriteLine(line, color);

    public static void Error(string? error, string? errorId = default) => CurHost.Error(error, errorId);

    public static void Warning(string? warning) => CurHost.Warning(warning);

    public static void Info(string? text) => CurHost.Info(text);

    public static void Trace(string? trace, string? origin = default) => CurHost.Trace(trace, origin);

    public static T GetService<T>() => CurHost.GetService<T>();
}