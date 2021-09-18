namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System;

    internal interface IProcess: IDisposable
    {
        // ReSharper disable once UnusedMethodReturnValue.Global
        ExitCode Run(IProcessListener listener);
    }
}