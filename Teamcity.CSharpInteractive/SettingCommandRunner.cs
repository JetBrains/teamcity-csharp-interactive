// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    internal class SettingCommandRunner<TOption>: ICommandRunner
        where TOption: struct, System.Enum
    {
        private readonly ILog<SettingCommandRunner<TOption>> _log;
        private readonly ISettingSetter<TOption> _settingSetter;

        public SettingCommandRunner(
            ILog<SettingCommandRunner<TOption>> log,
            ISettingSetter<TOption> settingSetter)
        {
            _log = log;
            _settingSetter = settingSetter;
        }

        public CommandResult TryRun(ICommand command)
        {
            if (command is not SettingCommand<TOption> settingCommand)
            {
                return new CommandResult(command, default);
            }

            var previousValue = _settingSetter.SetSetting(settingCommand.Value);
            var prevValueStr = previousValue.HasValue ? $" from {previousValue}" : string.Empty;
            _log.Trace(new []{new Text($"Change the {typeof(TOption).Name}{prevValueStr} to {settingCommand.Value}.")});
            return new CommandResult(command, true);
        }
    }
}