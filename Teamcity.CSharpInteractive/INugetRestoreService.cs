namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using NuGet.Versioning;

    internal interface INugetRestoreService
    {
        bool Restore(
            string packageId,
            NuGetVersion? version,
            IEnumerable<string> sources,
            IEnumerable<string> fallbackFolders,
            string outputPath,
            string packagesPath);
    }
}