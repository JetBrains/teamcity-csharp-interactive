namespace Teamcity.CSharpInteractive
{
    internal interface ICommandRunner
    {
        CommandResult TryRun(ICommand command);
    }
}