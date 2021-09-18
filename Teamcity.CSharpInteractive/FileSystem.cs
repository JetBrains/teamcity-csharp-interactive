// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    internal class FileSystem : IFileSystem
    {
        public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);
        
        public bool IsPathRooted(string path) => Path.IsPathRooted(path);

        public bool IsFileExist(string path) => File.Exists(path);
    }
}