namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using Contracts;

    internal interface INugetAssetsReader
    {
        IEnumerable<NuGetPackage> ReadPackages(string packagesPath, string assetsFilePath);
        
        IEnumerable<ReferencingAssembly> ReadReferencingAssemblies(string assetsFilePath);
    }
}