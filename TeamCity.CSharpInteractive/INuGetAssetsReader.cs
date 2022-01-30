namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface INuGetAssetsReader
{
    IEnumerable<NuGetPackage> ReadPackages(string packagesPath, string projectAssetsJson);

    IEnumerable<ReferencingAssembly> ReadReferencingAssemblies(string projectAssetsJson);
}