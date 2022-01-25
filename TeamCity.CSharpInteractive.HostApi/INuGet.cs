// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace HostApi;

public interface INuGet
{
    IEnumerable<NuGetPackage> Restore(NuGetRestore settings);

    IEnumerable<NuGetPackage> Restore(string packageId, string? versionRange = default, string? targetFrameworkMoniker = default, string? packagesPath = default);
}