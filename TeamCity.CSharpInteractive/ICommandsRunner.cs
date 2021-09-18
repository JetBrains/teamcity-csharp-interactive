namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ICommandsRunner
    {
        IEnumerable<CommandResult> Run(IEnumerable<ICommand> commands);
    }
}