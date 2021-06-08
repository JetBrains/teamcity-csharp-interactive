// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    internal class FileSystem : IFileSystem
    {
        public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);
    }
}