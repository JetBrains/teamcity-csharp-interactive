namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface IScriptContext
    {
        IDisposable OverrideScriptDirectory(string? scriptDirectory);
    }
}