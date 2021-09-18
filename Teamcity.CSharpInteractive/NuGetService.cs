// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Contracts;
    using NuGet.Versioning;

    internal class NuGetService: INuGet
    {
        private readonly ILog<NuGetService> _log;
        private readonly IFileSystem _fileSystem;
        private readonly IUniqueNameGenerator _uniqueNameGenerator;
        private readonly IEnvironment _environment;
        private readonly INugetEnvironment _nugetEnvironment;
        private readonly INugetRestoreService _nugetRestoreService;
        private readonly INugetAssetsReader _nugetAssetsReader;

        public NuGetService(
            ILog<NuGetService> log,
            IFileSystem fileSystem,
            IUniqueNameGenerator uniqueNameGenerator,
            IEnvironment environment,
            INugetEnvironment nugetEnvironment,
            INugetRestoreService nugetRestoreService,
            INugetAssetsReader nugetAssetsReader)
        {
            _log = log;
            _fileSystem = fileSystem;
            _uniqueNameGenerator = uniqueNameGenerator;
            _environment = environment;
            _nugetEnvironment = nugetEnvironment;
            _nugetRestoreService = nugetRestoreService;
            _nugetAssetsReader = nugetAssetsReader;
        }

        public IEnumerable<NuGetPackage> Restore(string packageId, string versionRange, string? packagesPath)
        {
            packagesPath ??= _nugetEnvironment.PackagesPath;
            var tempDirectory = _environment.GetPath(SpecialFolder.Temp);
            var outputPath = Path.Combine(tempDirectory, _uniqueNameGenerator.Generate());
            if (!string.IsNullOrWhiteSpace(packagesPath) && !_fileSystem.IsPathRooted(packagesPath))
            {
                var basePath = _environment.GetPath(SpecialFolder.Working);
                packagesPath = Path.Combine(basePath, packagesPath);
            }

            var restoreResult = _nugetRestoreService.Restore(
                packageId,
                VersionRange.Parse(versionRange),
                _nugetEnvironment.Sources,
                _nugetEnvironment.FallbackFolders,
                outputPath,
                packagesPath);

            if (restoreResult == false)
            {
                _log.Warning($"Cannot restore the NuGet package {packageId} {versionRange}.");
                return Enumerable.Empty<NuGetPackage>();
            }
            
            var assetsFilePath = Path.Combine(outputPath, "project.assets.json");
            return _nugetAssetsReader.ReadPackages(packagesPath, assetsFilePath);
        }
    }
}