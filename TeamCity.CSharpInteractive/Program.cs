namespace TeamCity.CSharpInteractive;

public class Program
{
    // ReSharper disable once UnusedMember.Global
    public static int Main()
    {
        try
        {
            return Composer.ResolveProgram().Run();
        }
        finally
        {
            Composer.FinalDispose();
        }
    }

    private readonly ILog<Program> _log;
    private readonly IEnumerable<IActive> _activeObjects;
    private readonly IInfo _info;
    private readonly ISettings _settings;
    private readonly IExitTracker _exitTracker;
    private readonly Func<IScriptRunner> _runner;
    private readonly IStatistics _statistics;

    internal Program(
        ILog<Program> log,
        IEnumerable<IActive> activeObjects,
        IInfo info,
        ISettings settings,
        IExitTracker exitTracker,
        Func<IScriptRunner> runner,
        IStatistics statistics)
    {
        _log = log;
        _activeObjects = activeObjects;
        _info = info;
        _settings = settings;
        _exitTracker = exitTracker;
        _runner = runner;
        _statistics = statistics;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    internal int Run()
    {
        if (_settings.ShowVersionAndExit)
        {
            _info.ShowVersion();
            return 0;
        }

        _info.ShowHeader();

        if (_settings.ShowHelpAndExit)
        {
            _info.ShowHelp();
            return 0;
        }

        using var exitToken = _exitTracker.Track();
        try
        {
            using (Disposable.Create(_activeObjects.Select(i => i.Activate()).ToArray()))
            {
                var result = _runner().Run();
                if (_statistics.Errors.Any())
                {
                    result = 1;
                }

                return result;
            }
        }
        catch (Exception error)
        {
            _log.Error(ErrorId.Unhandled, error);
            return 1;
        }
        finally
        {
            _info.ShowFooter();
        }
    }
}