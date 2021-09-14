namespace Teamcity.CSharpInteractive
{
    using Microsoft.CodeAnalysis.Scripting;

    internal interface IScriptOptionsFactory
    {
        ScriptOptions Create();
    }
}