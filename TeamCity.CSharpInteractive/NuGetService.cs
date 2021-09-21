// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Contracts;
    using NuGet.Versioning;

    internal class NuGetService: INuGet
    {
        private readonly ILog<NuGetService> _log;
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;
        private readonly INugetEnvironment _nugetEnvironment;
        private readonly INugetRestoreService _nugetRestoreService;
        private readonly INugetAssetsReader _nugetAssetsReader;
        private readonly ICleaner _cleaner;

        public NuGetService(
            ILog<NuGetService> log,
            IFileSystem fileSystem,
            IEnvironment environment,
            INugetEnvironment nugetEnvironment,
            INugetRestoreService nugetRestoreService,
            INugetAssetsReader nugetAssetsReader,
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

        public IEnumerable<NuGetPackage> Restore(string packageId, string? versionRange, string? packagesPath)
        {
            packagesPath ??= _nugetEnvironment.PackagesPath;
            if (!string.IsNullOrWhiteSpace(packagesPath) && !_fileSystem.IsPathRooted(packagesPath))
            {
                var basePath = _environment.GetPath(SpecialFolder.Working);
                packagesPath = Path.Combine(basePath, packagesPath);
            }
            
            var restoreResult = _nugetRestoreService.TryRestore(
                packageId,
                versionRange != default ? VersionRange.Parse(versionRange) : default,
                _nugetEnvironment.Sources,
                _nugetEnvironment.FallbackFolders,
                packagesPath,
                out var projectAssetsJson);

            if (restoreResult == false)
            {
                _log.Warning($"Cannot restore the NuGet package {packageId} {versionRange}.");
                return Enumerable.Empty<NuGetPackage>();
            }
            
            var output = Path.GetDirectoryName(projectAssetsJson);
            IDisposable outputPathToken = Disposable.Empty;
            if (!string.IsNullOrWhiteSpace(output))
            {
                outputPathToken = _cleaner.Track(output);
            }

            using (outputPathToken)
            {
                return _nugetAssetsReader.ReadPackages(packagesPath, projectAssetsJson);
            }
        }
    }
}