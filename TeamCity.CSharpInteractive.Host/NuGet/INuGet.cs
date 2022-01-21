// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Script.NuGet;

public interface INuGet
{
    IEnumerable<NuGetPackage> Restore(RestoreSettings settings);

    IEnumerable<NuGetPackage> Restore(string packageId, string? versionRange = default, string? targetFrameworkMoniker = default, string? packagesPath = default);
}