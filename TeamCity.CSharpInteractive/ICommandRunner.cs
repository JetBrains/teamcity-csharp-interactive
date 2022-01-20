namespace TeamCity.CSharpInteractive;

internal interface ICommandRunner
{
    CommandResult TryRun(ICommand command);
}