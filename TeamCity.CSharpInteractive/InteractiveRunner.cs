// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace TeamCity.CSharpInteractive;

internal class InteractiveRunner : IScriptRunner
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

    public int Run()
    {
        ShowCursor(true);
        // ReSharper disable once UseDeconstruction
        foreach (var result in _commandsRunner.Run(_commandSource.GetCommands()))
        {
            var exitCode = result.ExitCode;
            if (exitCode.HasValue)
            {
                return exitCode.Value;
            }

            if (!result.Command.Internal)
            {
                ShowCursor(result.Command is not CodeCommand);
            }
        }

        return 0;
    }

    private void ShowCursor(bool completed) =>
        _stdOut.Write(new Text(completed ? "> " : ". "));
}