// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;

    public interface INuGet
    {
        IEnumerable<NuGetPackage> Restore(string packageId, string? versionRange = null, string? targetFrameworkMoniker = null, string? packagesPath = null);
    }
}