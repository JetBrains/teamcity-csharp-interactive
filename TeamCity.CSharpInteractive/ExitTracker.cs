// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class ExitTracker : IExitTracker
{
    private readonly ISettings _settings;
    private readonly IEnvironment _environment;
    private readonly IPresenter<Summary> _summaryPresenter;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private volatile bool _isTerminating;

    public ExitTracker(
        ISettings settings,
        IEnvironment environment,
        IPresenter<Summary> summaryPresenter,
        CancellationTokenSource cancellationTokenSource)
    {
        _settings = settings;
        _environment = environment;
        _summaryPresenter = summaryPresenter;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public bool IsTerminating => _isTerminating;

    public IDisposable Track()
    {
        switch (_settings.InteractionMode)
        {
            case InteractionMode.Interactive:
                System.Console.CancelKeyPress += ConsoleOnCancelKeyPress;
                return Disposable.Create(() => System.Console.CancelKeyPress -= ConsoleOnCancelKeyPress);

            default:
                AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
                return Disposable.Create(() => AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnProcessExit);
        }
    }

    private void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        _isTerminating = true;

        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch
        {
            // ignored
        }
        
        _summaryPresenter.Show(Summary.Empty);
    }

    private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e) => _environment.Exit(0);
}