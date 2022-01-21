namespace TeamCity.CSharpInteractive;

using Script.NuGet;

internal interface INuGetAssetsReader
{
    IEnumerable<NuGetPackage> ReadPackages(string packagesPath, string projectAssetsJson);
        
    IEnumerable<ReferencingAssembly> ReadReferencingAssemblies(string projectAssetsJson);
}