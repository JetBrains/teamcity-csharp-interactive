namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface IActive
    {
        IDisposable Activate();
    }
}