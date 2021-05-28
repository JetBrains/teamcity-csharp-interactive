namespace Teamcity.CSharpInteractive
{
    internal interface ICommandRunner
    {
        bool? TryRun(ICommand command);
    }
}