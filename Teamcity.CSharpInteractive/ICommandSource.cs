namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ICommandSource
    {
        IEnumerable<ICommand> GetCommands();
    }
}