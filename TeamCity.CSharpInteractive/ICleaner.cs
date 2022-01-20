namespace TeamCity.CSharpInteractive;

internal interface ICleaner
{
    IDisposable Track(string path);
}