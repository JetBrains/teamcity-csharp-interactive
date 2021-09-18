namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System.Collections.Generic;

    internal interface IProcessResult
    {
        ExitCode ExitCode { get; }
        
        IReadOnlyCollection<string> StdOut { get; }
        
        IReadOnlyCollection<string> StdErr { get; }
    }
}