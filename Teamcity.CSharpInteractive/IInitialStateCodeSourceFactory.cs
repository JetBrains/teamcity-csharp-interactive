namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IInitialStateCodeSourceFactory
    {
        ICodeSource Create(IReadOnlyCollection<string> scriptArguments);
    }
}