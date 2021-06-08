// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    internal class CSharpScriptCommandRunner : ICommandRunner
    {
        private readonly ICSharpScriptRunner _scriptRunner;

        public CSharpScriptCommandRunner(ICSharpScriptRunner scriptRunner) =>
            _scriptRunner = scriptRunner;

        public CommandResult TryRun(ICommand command) =>
            command switch
            {
                ScriptCommand scriptCommand => new CommandResult(command, _scriptRunner.Run(scriptCommand.Script)),
                _ => new CommandResult(command, default)
            };
    }
}