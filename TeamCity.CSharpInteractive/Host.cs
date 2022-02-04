// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using HostApi;
using TeamCity.CSharpInteractive;
using Environment = System.Environment;

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
        AppDomain.CurrentDomain.DomainUnload += (_, _) => Finish();
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    }

    private static void Finish()
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
    
    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            Components.Log.Error(ErrorId.Exception, new[] {new Text(e.ExceptionObject.ToString() ?? "Unhandled exception.")});
            Finish();
            Environment.Exit(1);
        }
        catch
        {
            // ignored
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

    [Pure]
    public static T GetService<T>() => CurHost.GetService<T>();
}