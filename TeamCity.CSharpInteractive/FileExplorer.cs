// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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

        public IEnumerable<string> FindFiles(string searchPattern)
        {
            var dotnetHome = _hostEnvironment.GetEnvironmentVariable("DOTNET_HOME") ?? string.Empty;
            var dockerHome = _hostEnvironment.GetEnvironmentVariable("DOCKER_HOME") ?? string.Empty;
            var paths = _hostEnvironment.GetEnvironmentVariable("PATH")?.Split(";", StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();
            var allPaths = new [] {dotnetHome, dockerHome}.Concat(paths).Where(i => !string.IsNullOrWhiteSpace(i));
            return 
                from path in allPaths 
                where _fileSystem.IsDirectoryExist(path)
                from file in _fileSystem.EnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly)
                where _fileSystem.IsFileExist(file)
                select file;
        }
    }
}