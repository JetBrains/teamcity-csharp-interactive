namespace TeamCity.CSharpInteractive
{
    internal interface IFileSystem
    {
        void DeleteDirectory(string path, bool recursive);

        bool IsPathRooted(string path);

        bool IsFileExist(string path);
    }
}