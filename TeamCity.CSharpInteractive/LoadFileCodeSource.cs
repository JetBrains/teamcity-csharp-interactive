// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections;

internal class LoadFileCodeSource : ICodeSource
{
    private readonly IFilePathResolver _filePathResolver;
    private readonly IScriptContext _scriptContext;
    private string _fileName = "";

    public LoadFileCodeSource(
        IFilePathResolver filePathResolver,
        IScriptContext scriptContext)
    {
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
        return new LinesEnumerator(new List<string>{$"#load \"{_fileName}\""}, () => scope.Dispose());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}