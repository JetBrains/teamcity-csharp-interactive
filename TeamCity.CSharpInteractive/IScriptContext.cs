namespace TeamCity.CSharpInteractive;

internal interface IScriptContext
{
    IDisposable CreateScope(ICodeSource source);
}