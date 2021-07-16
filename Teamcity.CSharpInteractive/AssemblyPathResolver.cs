// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.IO;

    internal class AssemblyPathResolver : IAssemblyPathResolver
    {
        private readonly ILog<AssemblyPathResolver> _log;
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;

        public AssemblyPathResolver(
            ILog<AssemblyPathResolver> log,
            IEnvironment environment,
            IFileSystem fileSystem)
        {
            _log = log;
            _environment = environment;
            _fileSystem = fileSystem;
        }

        public bool TryResolve(string? assemblyPath, out string fullAssemblyPath)
        {
            if (!string.IsNullOrWhiteSpace(assemblyPath) && !_fileSystem.IsPathRooted(assemblyPath))
            {
                foreach (var path in GetPaths())
                {
                    fullAssemblyPath = Path.Combine(path, assemblyPath);
                    var isFileExist = _fileSystem.IsFileExist(fullAssemblyPath);
                    _log.Trace($"Try to find \"{assemblyPath}\" in \"{path}\": {isFileExist}.");
                    if (isFileExist)
                    {
                        return true;
                    }
                }

                _log.Error(ErrorId.File, $"Cannot find \"{assemblyPath}\".");
            }

            fullAssemblyPath = string.Empty;
            return false;
        }

        private IEnumerable<string> GetPaths()
        {
            yield return _environment.GetPath(SpecialFolder.Working);
        }
    }
}