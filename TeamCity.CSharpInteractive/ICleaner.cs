namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface ICleaner
    {
        IDisposable Track(string path);
    }
}