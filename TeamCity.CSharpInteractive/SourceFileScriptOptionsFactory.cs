namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

internal class SourceFileScriptOptionsFactory:  IScriptOptionsFactory
{
    private readonly IEnvironment _environment;

    public SourceFileScriptOptionsFactory(IEnvironment environment) =>
        _environment = environment;

    public ScriptOptions Create(ScriptOptions baseOptions) =>
        baseOptions.WithSourceResolver(new SourceFileResolver(GetSearchPaths(), Path.GetFullPath(_environment.GetPath(SpecialFolder.Script))));
    
    private IEnumerable<string> GetSearchPaths()
    {
        yield return Path.GetFullPath(_environment.GetPath(SpecialFolder.Script));
        yield return Path.GetFullPath(_environment.GetPath(SpecialFolder.Working));
    }
}