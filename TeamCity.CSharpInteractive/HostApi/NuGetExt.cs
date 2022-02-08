namespace HostApi;

using NuGet.Versioning;

public static class NuGetExt
{
    public static IEnumerable<NuGetPackage> Restore(this INuGet nuGet, string packageId, string? versionRange = default, string? targetFrameworkMoniker = default, string? packagesPath = default)
    {
        var settings = new NuGetRestoreSettings(packageId);
        if (versionRange != default)
        {
            settings = settings.WithVersionRange(VersionRange.Parse(versionRange));
        }

        if (targetFrameworkMoniker != default)
        {
            settings = settings.WithTargetFrameworkMoniker(targetFrameworkMoniker);
        }

        if (packagesPath != default)
        {
            settings = settings.WithPackagesPath(packagesPath);
        }

        return nuGet.Restore(settings);
    }
}