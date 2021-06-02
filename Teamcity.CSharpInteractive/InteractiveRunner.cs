// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Teamcity.CSharpInteractive
{
    internal class InteractiveRunner : IRunner
    {
        private readonly ICommandSource _commandSource;
        private readonly ICommandsRunner _commandsRunner;
        private readonly IStdOut _stdOut;
        
        public InteractiveRunner(
            ICommandSource commandSource,
            ICommandsRunner commandsRunner,
            IStdOut stdOut)
        {
            _commandSource = commandSource;
            _commandsRunner = commandsRunner;
            _stdOut = stdOut;
        }

        public InteractionMode InteractionMode => InteractionMode.Interactive;

        public ExitCode Run()
        {
            ShowCursor(true);
            foreach (var result in _commandsRunner.Run(_commandSource.GetCommands()))
            {
                if (result.Command.Kind == CommandKind.Code)
                {
                    ShowCursor(false);
                }
                else
                {
                    ShowCursor(true);
                }
            }

            return ExitCode.Success;
        }

        private void ShowCursor(bool completed) =>
            _stdOut.Write(new Text(completed ? "> " : ". "));
    }
}