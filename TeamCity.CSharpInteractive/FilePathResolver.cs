// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class FilePathResolver : IFilePathResolver
{
    private readonly ILog<FilePathResolver> _log;
    private readonly IEnvironment _environment;
    private readonly IFileSystem _fileSystem;

    public FilePathResolver(
        ILog<FilePathResolver> log,
        IEnvironment environment,
        IFileSystem fileSystem)
    {
        _log = log;
        _environment = environment;
        _fileSystem = fileSystem;
    }

    public bool TryResolve(string? fileOrDirectoryPath, out string fullScriptPath)
    {
        var state = State.Unknown;
        fullScriptPath = string.Empty;
        if (!string.IsNullOrWhiteSpace(fileOrDirectoryPath))
        {
            if (_fileSystem.IsPathRooted(fileOrDirectoryPath))
            {
                state = TryResolveFullPath(fileOrDirectoryPath, fileOrDirectoryPath, out fullScriptPath);
                if (state == State.NotFound)
                {
                    return false;
                }
            }
            else
            {
                foreach (var path in GetPaths().Distinct())
                {
                    fullScriptPath = Path.Combine(path, fileOrDirectoryPath);
                    state = TryResolveFullPath(path, fullScriptPath, out fullScriptPath);
                    if (state is State.Found or State.Error)
                    {
                        break;
                    }
                }
            }
        }

        if (state is State.NotFound)
        {
            _log.Error(ErrorId.CannotFind, $"Cannot find \"{fileOrDirectoryPath}\".");
        }
        
        return state == State.Found;
    }

    private State TryResolveFullPath(string basePath, string fullPath, out string fullScriptPath)
    {
        fullScriptPath = string.Empty;
        var isFileExist = _fileSystem.IsFileExist(fullPath);
        _log.Trace(() => new[] {new Text($"Try to find file \"{fullPath}\" in \"{basePath}\": {isFileExist}.")});
        if (isFileExist)
        {
            fullScriptPath = fullPath;
            return State.Found;
        }
                
        var isDirectoryExist = _fileSystem.IsDirectoryExist(fullPath);
        _log.Trace(() => new[] {new Text($"Try to find directory \"{fullPath}\" in \"{basePath}\": {isDirectoryExist}.")});
        if (isDirectoryExist)
        {
            var scripts = _fileSystem.EnumerateFileSystemEntries(fullPath, "*.csx", SearchOption.TopDirectoryOnly).ToList();
            switch (scripts.Count)
            {
                case 1:
                    fullScriptPath = scripts[0];
                    return State.Found;
                case > 1:
                    _log.Error(ErrorId.CannotFind, new Text($"Specify which script file to use because the folder \"{fullPath}\" contains more than one script file."));
                    return State.Error;
                default:
                    return State.Unknown;
            }

        }
        
        return State.NotFound;
    }
    
    private enum State
    {
        Unknown,
        Found,
        Error,
        NotFound
    }

    private IEnumerable<string> GetPaths()
    {
        yield return _environment.GetPath(SpecialFolder.Script);
        yield return _environment.GetPath(SpecialFolder.Working);
    }
}