namespace TeamCity.CSharpInteractive;

using NuGet.Versioning;

internal interface INuGetReferenceResolver
{
    bool TryResolveAssemblies(string packageId, VersionRange? versionRange, out IReadOnlyCollection<ReferencingAssembly> assemblies);
}