namespace Teamcity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;

    public interface INuGet
    {
        IEnumerable<NuGetPackage> Restore(string packageId, string versionRange, string? packagesPath = null);
    }
}