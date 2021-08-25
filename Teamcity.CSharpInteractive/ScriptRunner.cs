// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using Host;

    internal class ScriptRunner : IRunner
    {
        private readonly ILog<ScriptRunner> _log;
        private readonly ICommandSource _commandSource;
        private readonly ICommandsRunner _commandsRunner;
        private readonly IStatistics _statistics;
        private readonly IPresenter<IStatistics> _statisticsPresenter;

        public ScriptRunner(
            ILog<ScriptRunner> log,
            ICommandSource commandSource,
            ICommandsRunner commandsRunner,
            IStatistics statistics,
            IPresenter<IStatistics> statisticsPresenter)
        {
            _log = log;
            _commandSource = commandSource;
            _commandsRunner = commandsRunner;
            _statistics = statistics;
            _statisticsPresenter = statisticsPresenter;
        }
        
        public ExitCode Run()
        {
            var exitCode = ExitCode.Success;
            try
            {
                foreach (var result in _commandsRunner.Run(GetCommands()))
                {
                    if (result.Success.HasValue)
                    {
                        if (!result.Success.Value)
                        {
                            exitCode = ExitCode.Fail;
                            break;
                        }
                    }
                    else
                    {
                        _log.Error(ErrorId.NotSupported, $"{result.Command} is not supported.");
                    }
                }

                if (exitCode == ExitCode.Fail || _statistics.Errors.Count > 0)
                {
                    _log.Info(new Text("Running FAILED.", Color.Error));
                    return ExitCode.Fail;
                }

                _log.Info(new Text("Running succeeded.", Color.Success));
                return exitCode;
            }
            finally
            {
                _statisticsPresenter.Show(_statistics);
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
}