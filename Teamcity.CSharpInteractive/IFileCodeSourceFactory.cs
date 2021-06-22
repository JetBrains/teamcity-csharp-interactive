namespace Teamcity.CSharpInteractive
{
    internal interface IFileCodeSourceFactory
    {
        ICodeSource Create(string fileName, bool isRoot);
    }
}