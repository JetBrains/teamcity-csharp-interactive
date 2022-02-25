namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis.Scripting;

internal class MetadataResolverOptionsFactory:  IScriptOptionsFactory
{
    private readonly IEnvironment _environment;

    public MetadataResolverOptionsFactory(IEnvironment environment) =>
        _environment = environment;

    public ScriptOptions Create(ScriptOptions baseOptions) =>
        baseOptions.WithMetadataResolver(ScriptMetadataResolver.Default.WithSearchPaths(GetSearchPaths()).WithBaseDirectory(Path.GetFullPath(_environment.GetPath(SpecialFolder.Script))));
    
    private IEnumerable<string> GetSearchPaths()
    {
        yield return Path.GetFullPath(_environment.GetPath(SpecialFolder.Script));
        yield return Path.GetFullPath(_environment.GetPath(SpecialFolder.Working));
    }
}