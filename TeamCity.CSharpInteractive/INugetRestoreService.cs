namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using NuGet.Versioning;

    internal interface INugetRestoreService
    {
        bool TryRestore(
            string packageId,
            VersionRange? versionRange,
            string? targetFrameworkMoniker,
            IEnumerable<string> sources,
            IEnumerable<string> fallbackFolders,
            string packagesPath,
            out string projectAssetsJson);
    }
}