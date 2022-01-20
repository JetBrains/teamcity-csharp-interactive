namespace TeamCity.CSharpInteractive;

internal interface IFileCodeSourceFactory
{
    ICodeSource Create(string fileName);
}