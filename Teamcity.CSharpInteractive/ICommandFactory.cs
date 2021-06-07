namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ICommandFactory<in T>
    {
        IEnumerable<ICommand> Create(T data);
    }
}