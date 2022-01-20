namespace TeamCity.CSharpInteractive;

internal interface IExitTracker
{
    IDisposable Track();
}