namespace TeamCity.CSharpInteractive;

internal interface IFileSystem
{
    void DeleteDirectory(string path, bool recursive);

    bool IsPathRooted(string path);

    bool IsFileExist(string file);
        
    bool IsDirectoryExist(string path);

    IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption);

    IEnumerable<string> ReadAllLines(string file);

    IStreamReader OpenReader(string file);
}