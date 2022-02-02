namespace TeamCity.CSharpInteractive;

internal interface IProcessResultHandler
{
    void Handle<T>(ProcessResult result, Action<T>? handler);
}