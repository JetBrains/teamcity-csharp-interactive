// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf
namespace TeamCity.CSharpInteractive;

internal class CSharpScriptCommandRunner : ICommandRunner
{
    private readonly ICSharpScriptRunner _scriptRunner;

    public CSharpScriptCommandRunner(ICSharpScriptRunner scriptRunner) => _scriptRunner = scriptRunner;

    public CommandResult TryRun(ICommand command)
    {
        switch (command)
        {
            case ScriptCommand scriptCommand:
                return _scriptRunner.Run(command, scriptCommand.Script);

            case ResetCommand:
                _scriptRunner.Reset();
                return new CommandResult(command, true);

            default:
                return new CommandResult(command, default);
        }
    }
}