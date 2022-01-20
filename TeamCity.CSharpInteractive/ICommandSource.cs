namespace TeamCity.CSharpInteractive;

internal interface ICommandSource
{
    IEnumerable<ICommand> GetCommands();
}