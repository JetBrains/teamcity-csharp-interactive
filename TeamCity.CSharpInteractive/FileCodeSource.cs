// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections;

internal class FileCodeSource : ICodeSource
{
    private readonly ILog<FileCodeSource> _log;
    private readonly IFileSystem _fileSystem;
    private readonly IFilePathResolver _filePathResolver;
    private readonly IScriptContext _scriptContext;
    private string _fileName = "";

    public FileCodeSource(
        ILog<FileCodeSource> log,
        IFileSystem fileSystem,
        IFilePathResolver filePathResolver,
        IScriptContext scriptContext)
    {
        _log = log;
        _fileSystem = fileSystem;
        _filePathResolver = filePathResolver;
        _scriptContext = scriptContext;
    }
    
    public string Name
    {
        get => _fileName;
        set
        {
            if (!_filePathResolver.TryResolve(value, out var fullFilePath))
            {
                fullFilePath = value;
            }

            _fileName = fullFilePath;
        }
    }

    public bool Internal => false;

    public IEnumerator<string> GetEnumerator()
    {
        var scope = _scriptContext.CreateScope(this);
        try
        {
            _log.Trace(() => new[] {new Text($@"Reading file ""{Name}"".")});
            return new LinesEnumerator(_fileSystem.ReadAllLines(Name).GetEnumerator(), () => scope.Dispose());
        }
        catch (Exception e)
        {
            _log.Error(ErrorId.File, new[] {new Text(e.Message)});
            return Enumerable.Empty<string>().GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class LinesEnumerator : IEnumerator<string>
    {
        private readonly IEnumerator<string> _baseEnumerator;
        private readonly Action _onDispose;

        public LinesEnumerator(IEnumerator<string> baseEnumerator, Action onDispose)
        {
            _baseEnumerator = baseEnumerator;
            _onDispose = onDispose;
        }

        public bool MoveNext() => _baseEnumerator.MoveNext();

        public void Reset() => _baseEnumerator.Reset();

        public string Current => _baseEnumerator.Current;

        object? IEnumerator.Current => ((IEnumerator)_baseEnumerator).Current;

        public void Dispose()
        {
            try
            {
                _baseEnumerator.Dispose();
            }
            finally
            {
                _onDispose();
            }
        }
    }
}