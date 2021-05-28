// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    internal class HelpCommandRunner: ICommandRunner
    {
        private readonly IInfo _info;

        public HelpCommandRunner(IInfo info) => _info = info;

        public bool? TryRun(ICommand command)
        {
            if (command.Kind != CommandKind.Help || command is not HelpCommand)
            {
                return null;
            }
            
            _info.ShowReplHelp();
            return true;
        }
    }
}