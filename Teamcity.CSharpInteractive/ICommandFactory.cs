namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ICommandFactory<in T>
    {
        int Order { get; }

        IEnumerable<ICommand> Create(T data);
    }
}