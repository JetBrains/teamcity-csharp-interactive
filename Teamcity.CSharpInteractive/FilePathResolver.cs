// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.IO;

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

        public bool TryResolve(string? filePath, out string fullFilePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && !_fileSystem.IsPathRooted(filePath))
            {
                foreach (var path in GetPaths())
                {
                    fullFilePath = Path.Combine(path, filePath);
                    var isFileExist = _fileSystem.IsFileExist(fullFilePath);
                    _log.Trace($"Try to find \"{filePath}\" in \"{path}\": {isFileExist}.");
                    if (isFileExist)
                    {
                        return true;
                    }
                }

                _log.Error(ErrorId.File, $"Cannot find \"{filePath}\".");
            }

            fullFilePath = string.Empty;
            return false;
        }

        private IEnumerable<string> GetPaths()
        {
            yield return _environment.GetPath(SpecialFolder.Script);
            yield return _environment.GetPath(SpecialFolder.Working);
        }
    }
}