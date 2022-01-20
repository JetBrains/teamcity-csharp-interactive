namespace TeamCity.CSharpInteractive;

internal interface IActive
{
    IDisposable Activate();
}