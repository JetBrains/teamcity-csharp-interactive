namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface IActive
    {
        IDisposable Activate();
    }
}