// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

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
        _log.Trace(() => new[] {new Text($"Start tracking \"{path}\".")});
        return Disposable.Create(() =>
        {
            _log.Trace(() => new[] {new Text($"Delete \"{path}\".")});
            _fileSystem.DeleteDirectory(path, true);
        });
    }
}