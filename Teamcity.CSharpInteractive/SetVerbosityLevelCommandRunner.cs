// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    internal class SetVerbosityLevelCommandRunner: ICommandRunner
    {
        private readonly ILog<SetVerbosityLevelCommandRunner> _log;
        private readonly ISettings _settings;

        public SetVerbosityLevelCommandRunner(
            ILog<SetVerbosityLevelCommandRunner> log,
            ISettings settings)
        {
            _log = log;
            _settings = settings;
        }

        public CommandResult TryRun(ICommand command)
        {
            if (command is not SetVerbosityLevelCommand setVerbosityLevelCommand)
            {
                return new CommandResult(command, default);
            }

            _log.Trace(new []{new Text($"Change the verbosity level from {_settings.VerbosityLevel} to {setVerbosityLevelCommand.VerbosityLevel}.")});
            _settings.VerbosityLevel = setVerbosityLevelCommand.VerbosityLevel;
            return new CommandResult(command, true);
        }
    }
}