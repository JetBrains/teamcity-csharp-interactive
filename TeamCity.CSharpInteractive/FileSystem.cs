// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Microsoft.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class FileSystem : IFileSystem
    {
        public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);
        
        public bool IsPathRooted(string path) => Path.IsPathRooted(path);

        public bool IsFileExist(string path) => File.Exists(path);

        public bool IsDirectoryExist(string path) => Directory.Exists(path);

        public void WriteAllLines(string path, IEnumerable<string> contents) => File.WriteAllLines(path, contents);

        public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
    }
}