namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

internal class MetadataResolverOptionsFactory: IScriptOptionsFactory
{
    private readonly Func<MetadataReferenceResolver> _metadataResolverFactory;

    public MetadataResolverOptionsFactory(Func<MetadataReferenceResolver> metadataResolverFactory) => _metadataResolverFactory = metadataResolverFactory;

    public ScriptOptions Create(ScriptOptions baseOptions) =>
        baseOptions.WithMetadataResolver(_metadataResolverFactory());
}