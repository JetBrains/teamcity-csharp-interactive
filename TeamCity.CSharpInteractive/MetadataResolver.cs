// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace TeamCity.CSharpInteractive;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

internal class MetadataResolver: MetadataReferenceResolver
{
    private static readonly ImmutableArray<PortableExecutableReference> NuGetReferences =
        ImmutableArray<PortableExecutableReference>.Empty
            .Add(MetadataReference.CreateFromFile(typeof(MetadataResolver).Assembly.Location));

    private readonly Lazy<ScriptMetadataResolver> _baseResolver;

    public MetadataResolver(IEnvironment environment) =>
        _baseResolver = new Lazy<ScriptMetadataResolver>(() => CreateResolver(environment));

    public override bool ResolveMissingAssemblies => _baseResolver.Value.ResolveMissingAssemblies;

    public override PortableExecutableReference? ResolveMissingAssembly(MetadataReference definition, AssemblyIdentity referenceIdentity) =>
        _baseResolver.Value.ResolveMissingAssembly(definition, referenceIdentity);

    public override bool Equals(object? other) => _baseResolver.Equals(other);

    public override int GetHashCode() => _baseResolver.GetHashCode();

    public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string? baseFilePath, MetadataReferenceProperties properties) =>
        reference.StartsWith("nuget:", StringComparison.CurrentCultureIgnoreCase)
            ? NuGetReferences
            : ShouldResolveReferenceInternal(reference, baseFilePath, properties);

    protected virtual ImmutableArray<PortableExecutableReference> ShouldResolveReferenceInternal(string reference, string? baseFilePath, MetadataReferenceProperties properties) => _baseResolver.Value.ResolveReference(reference, baseFilePath, properties);

    internal IEnumerable<string> SearchPaths => _baseResolver.Value.SearchPaths;

    internal string BaseDirectory => _baseResolver.Value.BaseDirectory;

    private static ScriptMetadataResolver CreateResolver(IEnvironment environment)
    {
        var scriptDirectory = Path.GetFullPath(environment.GetPath(SpecialFolder.Script));
        var workingDirectory = Path.GetFullPath(environment.GetPath(SpecialFolder.Working));
        return ScriptMetadataResolver.Default
            .WithSearchPaths(scriptDirectory, workingDirectory)
            .WithBaseDirectory(scriptDirectory);
    }
}