// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class FileSystem : IFileSystem
{
    public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);

    public bool IsPathRooted(string path) => Path.IsPathRooted(path);

    public bool IsFileExist(string file) => File.Exists(file);

    public bool IsDirectoryExist(string path) => Directory.Exists(path);

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    public IEnumerable<string> ReadAllLines(string file) => File.ReadAllLines(file);

    public IStreamReader OpenReader(string file) => new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
}