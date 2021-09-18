// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.IO;

    internal class AddNuGetReferenceCommandRunner: ICommandRunner
    {
        private readonly ILog<AddNuGetReferenceCommandRunner> _log;
        private readonly IEnvironment _environment;
        private readonly IUniqueNameGenerator _uniqueNameGenerator;
        private readonly INugetEnvironment _nugetEnvironment;
        private readonly INugetRestoreService _nugetRestoreService;
        private readonly INugetAssetsReader _nugetAssetsReader;
        private readonly ICleaner _cleaner;
        private readonly IReferenceRegistry _referenceRegistry;

        public AddNuGetReferenceCommandRunner(
            ILog<AddNuGetReferenceCommandRunner> log,
            IEnvironment environment,
            IUniqueNameGenerator uniqueNameGenerator,
            INugetEnvironment nugetEnvironment,
            INugetRestoreService nugetRestoreService,
            INugetAssetsReader nugetAssetsReader,
            ICleaner cleaner,
            IReferenceRegistry referenceRegistry)
        {
            _log = log;
            _environment = environment;
            _uniqueNameGenerator = uniqueNameGenerator;
            _nugetEnvironment = nugetEnvironment;
            _nugetRestoreService = nugetRestoreService;
            _nugetAssetsReader = nugetAssetsReader;
            _cleaner = cleaner;
            _referenceRegistry = referenceRegistry;
        }

        public CommandResult TryRun(ICommand command)
        {
            if (command is not AddNuGetReferenceCommand addPackageReferenceCommand)
            {
                return new CommandResult(command, default);
            }

            using var restoreToken = _log.Block($"Restore {addPackageReferenceCommand.PackageId} {addPackageReferenceCommand.VersionRange}".Trim());
            var tempDirectory = _environment.GetPath(SpecialFolder.Temp);
            var outputPath = Path.Combine(tempDirectory, _uniqueNameGenerator.Generate());
            var restoreResult = _nugetRestoreService.Restore(
                addPackageReferenceCommand.PackageId,
                addPackageReferenceCommand.VersionRange,
                _nugetEnvironment.Sources,
                _nugetEnvironment.FallbackFolders,
                outputPath,
                _nugetEnvironment.PackagesPath);

            if (!restoreResult)
            {
                return new CommandResult(command, false);
            }

            using var outputPathToken = _cleaner.Track(outputPath);
            using var addRefsToken = _log.Block("References");
            var assetsFilePath = Path.Combine(outputPath, "project.assets.json");
            var success = true;
            foreach (var assembly in _nugetAssetsReader.ReadReferencingAssemblies(assetsFilePath))
            {
                if (_referenceRegistry.TryRegisterAssembly(assembly.FilePath, out var description))
                {
                    _log.Info(assembly.Name);
                }
                else
                {
                    _log.Error(ErrorId.Nuget, $"Cannot add the reference \"{assembly.Name}\": {description}");
                    success = false;
                }
            }
            
            return new CommandResult(command, success);
        }
    }
}