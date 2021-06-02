// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    internal class HelpCommandRunner: ICommandRunner
    {
        private readonly IInfo _info;

        public HelpCommandRunner(IInfo info) => _info = info;

        public CommandResult TryRun(ICommand command)
        {
            if (command.Kind != CommandKind.Help || command is not HelpCommand)
            {
                return new CommandResult(command, default);
            }

            _info.ShowReplHelp();
            return new CommandResult(command, true);
        }
    }
}