namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System.Collections.Generic;
    using System.IO;

    internal class FileSystem : IFileSystem
    {
        public string CreateTempFilePath() => Path.GetTempFileName();

        public void DeleteFile(string file) => File.Delete(file);

        public void AppendAllLines(string file, IEnumerable<string> lines) =>
            File.AppendAllLines(file, lines);
    }
}