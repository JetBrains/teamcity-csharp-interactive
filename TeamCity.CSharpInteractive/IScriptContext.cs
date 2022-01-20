namespace TeamCity.CSharpInteractive;

internal interface IScriptContext
{
    IDisposable OverrideScriptDirectory(string? scriptDirectory);
}