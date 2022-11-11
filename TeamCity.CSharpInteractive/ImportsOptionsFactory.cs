namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis.Scripting;

internal class ImportsOptionsFactory: IScriptOptionsFactory
{
    public ScriptOptions Create(ScriptOptions baseOptions) =>
#if APPLICATION
        baseOptions.AddImports("Host");
#else
        baseOptions.AddImports("Components");
#endif
}