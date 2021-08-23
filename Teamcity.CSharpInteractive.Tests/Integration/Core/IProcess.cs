namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System;

    internal interface IProcess: IDisposable
    {
        ExitCode Run(IProcessListener listener);
    }
}