namespace TeamCity.CSharpInteractive
{
    using Microsoft.CodeAnalysis.Scripting;

    internal interface IScriptOptionsFactory
    {
        ScriptOptions Create(ScriptOptions baseOptions);
    }
}