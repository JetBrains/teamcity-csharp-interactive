namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

internal class SourceFileScriptOptionsFactory : IScriptOptionsFactory
{
    private readonly Func<SourceReferenceResolver> _sourceReferenceResolverFactory;

    public SourceFileScriptOptionsFactory(Func<SourceReferenceResolver> sourceReferenceResolverFactory) =>
        _sourceReferenceResolverFactory = sourceReferenceResolverFactory;

    public ScriptOptions Create(ScriptOptions baseOptions) =>
        baseOptions.WithSourceResolver(_sourceReferenceResolverFactory());
}