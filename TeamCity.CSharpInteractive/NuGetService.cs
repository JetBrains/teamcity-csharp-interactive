// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;
using NuGet.Versioning;

internal class NuGetService : INuGet
{
    private readonly ILog<NuGetService> _log;
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironment _environment;
    private readonly INuGetEnvironment _nugetEnvironment;
    private readonly INuGetRestoreService _nugetRestoreService;
    private readonly INuGetAssetsReader _nugetAssetsReader;
    private readonly ICleaner _cleaner;

    public NuGetService(
        ILog<NuGetService> log,
        IFileSystem fileSystem,
        IEnvironment environment,
        INuGetEnvironment nugetEnvironment,
        INuGetRestoreService nugetRestoreService,
        INuGetAssetsReader nugetAssetsReader,
        ICleaner cleaner)
    {
        _log = log;
        _fileSystem = fileSystem;
        _environment = environment;
        _nugetEnvironment = nugetEnvironment;
        _nugetRestoreService = nugetRestoreService;
        _nugetAssetsReader = nugetAssetsReader;
        _cleaner = cleaner;
    }

    public IEnumerable<NuGetPackage> Restore(NuGetRestoreSettings settings)
    {
        var packagesPath = settings.PackagesPath;
        if (string.IsNullOrWhiteSpace(packagesPath))
        {
            packagesPath = _nugetEnvironment.PackagesPath;
        }

        if (!_fileSystem.IsPathRooted(packagesPath))
        {
            packagesPath = Path.Combine(_environment.GetPath(SpecialFolder.Working), packagesPath);
        }

        settings = settings.WithPackagesPath(packagesPath);
        if (!settings.Sources.Any())
        {
            settings = settings.WithSources(_nugetEnvironment.Sources);
        }

        if (!settings.FallbackFolders.Any())
        {
            settings = settings.WithFallbackFolders(_nugetEnvironment.FallbackFolders);
        }

        var restoreResult = _nugetRestoreService.TryRestore(settings, out var projectAssetsJson);
        if (restoreResult == false)
        {
            _log.Warning($"Cannot restore the NuGet package {settings.PackageId} {settings.VersionRange}".Trim() + '.');
            return Enumerable.Empty<NuGetPackage>();
        }

        var output = Path.GetDirectoryName(projectAssetsJson);
        var outputPathToken = Disposable.Empty;
        if (!string.IsNullOrWhiteSpace(output))
        {
            outputPathToken = _cleaner.Track(output);
        }

        using (outputPathToken)
        {
            return _nugetAssetsReader.ReadPackages(packagesPath, projectAssetsJson);
        }
    }

    public IEnumerable<NuGetPackage> Restore(string packageId, string? versionRange, string? targetFrameworkMoniker, string? packagesPath)
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

        return Restore(settings);
    }
}