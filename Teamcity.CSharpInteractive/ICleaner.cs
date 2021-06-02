namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface ICleaner
    {
        IDisposable Track(string path);
    }
}