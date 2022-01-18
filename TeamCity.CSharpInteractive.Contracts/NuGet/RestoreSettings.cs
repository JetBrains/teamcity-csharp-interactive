// ReSharper disable CheckNamespace
namespace NuGet;

using System.Collections.Generic;
using System.Linq;

[Immutype.Target]
public record RestoreSettings(
    string PackageId,
    IEnumerable<string> Sources,
    IEnumerable<string> FallbackFolders,
    Versioning.VersionRange? VersionRange = default,
    string? TargetFrameworkMoniker = default,
    string? PackagesPath = default,
    PackageType? PackageType = default,
    bool? DisableParallel = default,
    bool? IgnoreFailedSources = default,
    bool? HideWarningsAndErrors = default,
    bool? NoCache = default)
{
    public RestoreSettings(string packageId)
        : this(packageId, Enumerable.Empty<string>(), Enumerable.Empty<string>())
    { }
}