namespace HostApi;

using Immutype;
using NuGet.Versioning;

[Target]
public record NuGetRestore(
    string PackageId,
    IEnumerable<string> Sources,
    IEnumerable<string> FallbackFolders,
    VersionRange? VersionRange = default,
    string? TargetFrameworkMoniker = default,
    string? PackagesPath = default,
    NuGetPackageType? PackageType = default,
    bool? DisableParallel = default,
    bool? IgnoreFailedSources = default,
    bool? HideWarningsAndErrors = default,
    bool? NoCache = default)
{
    public NuGetRestore(string packageId)
        : this(packageId, Enumerable.Empty<string>(), Enumerable.Empty<string>())
    { }
}