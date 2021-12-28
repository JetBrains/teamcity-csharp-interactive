// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    internal class FileSystem : IFileSystem
    {
        public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);
        
        public bool IsPathRooted(string path) => Path.IsPathRooted(path);

        public bool IsFileExist(string file) => File.Exists(file);

        public bool IsDirectoryExist(string path) => Directory.Exists(path);

        public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

        public IEnumerable<string> ReadAllLines(string file) => File.ReadAllLines(file);

        public IFileReader OpenReader(string file) => new FileReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    }
}