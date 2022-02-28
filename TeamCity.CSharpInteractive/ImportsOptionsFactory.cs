namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis.Scripting;

internal class ImportsOptionsFactory: IScriptOptionsFactory
{
    public ScriptOptions Create(ScriptOptions baseOptions) => baseOptions.AddImports("Host");
}