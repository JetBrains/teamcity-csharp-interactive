// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    internal class ScriptRunner : IRunner
    {
        private readonly ILog<ScriptRunner> _log;
        private readonly ICommandSource _commandSource;
        private readonly ICommandRunner[] _commandRunners;
        private readonly IStatistics _statistics;
        private readonly IPresenter<IStatistics> _statisticsPresenter;

        public ScriptRunner(
            ILog<ScriptRunner> log,
            ICommandSource commandSource,
            ICommandRunner[] commandRunners,
            IStatistics statistics,
            IPresenter<IStatistics> statisticsPresenter)
        {
            _log = log;
            _commandSource = commandSource;
            _commandRunners = commandRunners;
            _statistics = statistics;
            _statisticsPresenter = statisticsPresenter;
        }
        
        public InteractionMode InteractionMode => InteractionMode.Script;

        public ExitCode Run()
        {
            using (_statistics.Start())
            {
                foreach (var command in _commandSource.GetCommands())
                {
                    using var blockToken = _log.Block(new[] {new Text(command.Name)});
                    foreach (var runner in _commandRunners)
                    {
                        var result = command.Kind switch
                        {
                            CommandKind.Code => true,
                            _ => runner.TryRun(command)
                        };

                        if (result.HasValue)
                        {
                            break;
                        }
                    }
                }
            }

            if (_statistics.Errors.Count > 0)
            {
                _log.Info(new []{ Text.NewLine, new Text("Running FAILED.", Color.Error)});
                _statisticsPresenter.Show(_statistics);
                return ExitCode.Fail;
            }

            _log.Info(new []{ Text.NewLine, new Text("Running succeeded.", Color.Success)});
            _statisticsPresenter.Show(_statistics);
            return ExitCode.Success;
        }
    }
}