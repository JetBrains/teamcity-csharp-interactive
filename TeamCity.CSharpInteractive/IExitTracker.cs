namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface IExitTracker
    {
        IDisposable Track();
    }
}