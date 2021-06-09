// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    internal class CSharpScriptCommandRunner : ICommandRunner
    {
        private readonly ICSharpScriptRunner _scriptRunner;

        public CSharpScriptCommandRunner(ICSharpScriptRunner scriptRunner) =>
            _scriptRunner = scriptRunner;

        public CommandResult TryRun(ICommand command)
        {
            switch (command)
            {
                case ScriptCommand scriptCommand:
                    return new CommandResult(command, _scriptRunner.Run(scriptCommand.Script));

                case ResetCommand:
                    _scriptRunner.Reset();
                    return new CommandResult(command, true);

                default:
                    return new CommandResult(command, default);
            }
        }
    }
}