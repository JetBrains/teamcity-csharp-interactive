// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.IO;

    internal class Cleaner : ICleaner
    {
        private readonly ILog<Cleaner> _log;

        public Cleaner(ILog<Cleaner> log) => _log = log;

        public IDisposable Track(string path)
        {
            _log.Trace($"Trace \"{path}\".");
            return Disposable.Create(() =>
            {
                _log.Trace($"Delete \"{path}\".");
                Directory.Delete(path, true);
            });
        }
    }
}