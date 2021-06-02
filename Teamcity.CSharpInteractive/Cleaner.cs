// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.IO;

    internal class Cleaner : ICleaner
    {
        public IDisposable Track(string path) => Disposable.Create(() => Directory.Delete(path, true));
    }
}