// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf
namespace TeamCity.CSharpInteractive;

internal class ScriptRunner : IScriptRunner
{
    private readonly ILog<ScriptRunner> _log;
    private readonly ICommandSource _commandSource;
    private readonly ICommandsRunner _commandsRunner;
    private readonly IStatistics _statistics;
    private readonly IPresenter<Summary> _summaryPresenter;

    public ScriptRunner(
        ILog<ScriptRunner> log,
        ICommandSource commandSource,
        ICommandsRunner commandsRunner,
        IStatistics statistics,
        IPresenter<Summary> summaryPresenter)
    {
        _log = log;
        _commandSource = commandSource;
        _commandsRunner = commandsRunner;
        _statistics = statistics;
        _summaryPresenter = summaryPresenter;
    }

    public int Run()
    {
        var summary = new Summary(true);
        try
        {
            int? exitCode = default;
            foreach (var (command, success, currentExitCode) in _commandsRunner.Run(GetCommands()))
            {
                if (success.HasValue)
                {
                    if (!success.Value)
                    {
                        summary = summary.WithSuccess(false);
                        break;
                    }
                }
                else
                {
                    _log.Error(ErrorId.NotSupported, $"{command} is not supported.");
                }

                exitCode = currentExitCode;
            }

            return exitCode ?? (summary.Success == false || _statistics.Errors.Count > 0 ? 1 : 0);
        }
        finally
        {
            _summaryPresenter.Show(summary);
        }
    }

    private IEnumerable<ICommand> GetCommands()
    {
        CodeCommand? codeCommand = null;
        foreach (var command in _commandSource.GetCommands())
        {
            codeCommand = command as CodeCommand;
            if (codeCommand == null)
            {
                yield return command;
            }
        }

        if (codeCommand != null)
        {
            _log.Error(ErrorId.UncompletedScript, "Script is uncompleted.");
        }
    }
}