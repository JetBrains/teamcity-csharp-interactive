using NuGet;
using NuGet.Versioning;

// Package version
NuGetVersion GetNextVersion(RestoreSettings settings) =>
    GetService<INuGet>()
        .Restore(settings.WithHideWarningsAndErrors(true))
        .Where(i => i.Name == settings.PackageId)
        .Select(i => i.NuGetVersion)
        .Select(i => new NuGetVersion(i.Major, i.Minor, i.Patch + 1))
        .DefaultIfEmpty(string.IsNullOrWhiteSpace(Props["version"]) ? new NuGetVersion(1, 0, 0) : new NuGetVersion(Props["version"]))
        .Max()!;

var packageRestoreSettings = new RestoreSettings("TeamCity.CSharpInteractive").WithVersionRange(VersionRange.All);
var nextPackageVersion = GetNextVersion(packageRestoreSettings);

var toolRestoreSettings = new RestoreSettings("TeamCity.csi").WithPackageType(PackageType.Tool).WithVersionRange(VersionRange.All);
var nextToolVersion = GetNextVersion(toolRestoreSettings);

WriteLine(nextPackageVersion);
WriteLine(nextToolVersion);