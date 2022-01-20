// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class FileExplorer : IFileExplorer
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IFileSystem _fileSystem;

    public FileExplorer(
        IHostEnvironment hostEnvironment,
        IFileSystem fileSystem)
    {
        _hostEnvironment = hostEnvironment;
        _fileSystem = fileSystem;
    }

    public IEnumerable<string> FindFiles(string searchPattern, params string[] additionalVariables)
    {
        var additionalPaths = additionalVariables.Select(varName => _hostEnvironment.GetEnvironmentVariable(varName));
        var paths = _hostEnvironment.GetEnvironmentVariable("PATH")?.Split(";", StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();
        var allPaths = additionalPaths.Concat(paths).Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i?.Trim()).Distinct();
        return (
                from path in allPaths 
                where _fileSystem.IsDirectoryExist(path)
                from file in _fileSystem.EnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly)
                where _fileSystem.IsFileExist(file)
                select file)
            .Distinct();
    }
}