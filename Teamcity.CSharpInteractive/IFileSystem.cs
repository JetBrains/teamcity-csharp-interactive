namespace Teamcity.CSharpInteractive
{
    internal interface IFileSystem
    {
        void DeleteDirectory(string path, bool recursive);
    }
}