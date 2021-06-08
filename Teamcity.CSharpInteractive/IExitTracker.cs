namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface IExitTracker
    {
        IDisposable Track();
    }
}