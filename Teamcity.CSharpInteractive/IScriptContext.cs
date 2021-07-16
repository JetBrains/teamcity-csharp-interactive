namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface IScriptContext
    {
        IDisposable OverrideScriptDirectory(string? scriptDirectory);
    }
}