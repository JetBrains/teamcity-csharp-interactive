namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface IWorkingDirectoryContext
    {
        IDisposable OverrideWorkingDirectory(string? workingDirectory);
    }
}