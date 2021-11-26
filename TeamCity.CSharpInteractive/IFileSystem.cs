namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IFileSystem
    {
        void DeleteDirectory(string path, bool recursive);

        bool IsPathRooted(string path);

        bool IsFileExist(string path);

        void WriteAllLines(string path, IEnumerable<string> contents);
    }
}