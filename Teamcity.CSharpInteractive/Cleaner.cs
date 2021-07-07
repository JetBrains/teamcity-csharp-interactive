// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using Host;

    internal class Cleaner : ICleaner
    {
        private readonly ILog<Cleaner> _log;
        private readonly IFileSystem _fileSystem;

        public Cleaner(ILog<Cleaner> log, IFileSystem fileSystem)
        {
            _log = log;
            _fileSystem = fileSystem;
        }

        public IDisposable Track(string path)
        {
            _log.Trace($"Trace \"{path}\".");
            return Disposable.Create(() =>
            {
                _log.Trace($"Delete \"{path}\".");
                _fileSystem.DeleteDirectory(path, true);
            });
        }
    }
}