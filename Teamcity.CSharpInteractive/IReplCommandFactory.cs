namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IReplCommandFactory
    {
        IEnumerable<ICommand> TryCreate(string replCommand);
    }
}