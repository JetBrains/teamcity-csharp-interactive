namespace TeamCity.CSharpInteractive;

internal interface IExitTracker
{
    bool IsTerminating { get; }
    
    IDisposable Track();
}