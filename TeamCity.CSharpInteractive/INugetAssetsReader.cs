namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Contracts;

    internal interface INugetAssetsReader
    {
        IEnumerable<NuGetPackage> ReadPackages(string packagesPath, string projectAssetsJson);
        
        IEnumerable<ReferencingAssembly> ReadReferencingAssemblies(string projectAssetsJson);
    }
}