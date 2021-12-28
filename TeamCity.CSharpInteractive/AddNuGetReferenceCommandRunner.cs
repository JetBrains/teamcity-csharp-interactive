// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.IO;
    using Contracts;

    internal class AddNuGetReferenceCommandRunner: ICommandRunner
    {
        private readonly ILog<AddNuGetReferenceCommandRunner> _log;
        private readonly INugetEnvironment _nugetEnvironment;
        private readonly INugetRestoreService _nugetRestoreService;
        private readonly INugetAssetsReader _nugetAssetsReader;
        private readonly ICleaner _cleaner;
        private readonly IReferenceRegistry _referenceRegistry;

        public AddNuGetReferenceCommandRunner(
            ILog<AddNuGetReferenceCommandRunner> log,
            INugetEnvironment nugetEnvironment,
            INugetRestoreService nugetRestoreService,
            INugetAssetsReader nugetAssetsReader,
            ICleaner cleaner,
            IReferenceRegistry referenceRegistry)
        {
            _log = log;
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

            var packageName = $"{addPackageReferenceCommand.PackageId} {addPackageReferenceCommand.VersionRange}".Trim();
            var success = true;
            using (_log.Block(new[] { new Text($"Restoring package {packageName}.", Color.Highlighted) }))
            {
                var restoreResult = _nugetRestoreService.TryRestore(
                    addPackageReferenceCommand.PackageId,
                    addPackageReferenceCommand.VersionRange,
                    default,
                    _nugetEnvironment.Sources,
                    _nugetEnvironment.FallbackFolders,
                    _nugetEnvironment.PackagesPath,
                    out var projectAssetsJson);

                if (!restoreResult)
                {
                    return new CommandResult(command, false);
                }

                var output = Path.GetDirectoryName(projectAssetsJson);
                var outputPathToken = Disposable.Empty;
                if (!string.IsNullOrWhiteSpace(output))
                {
                    outputPathToken = _cleaner.Track(output);
                }

                using (outputPathToken)
                {
                    _log.Info(new[] { new Text("References:", Color.Details) });
                    foreach (var assembly in _nugetAssetsReader.ReadReferencingAssemblies(projectAssetsJson))
                    {
                        if (_referenceRegistry.TryRegisterAssembly(assembly.FilePath, out var description))
                        {
                            _log.Info(Text.Tab, new Text(assembly.Name, Color.Details));
                        }
                        else
                        {
                            _log.Error(ErrorId.Nuget, $"Cannot add the reference \"{assembly.Name}\": {description}");
                            success = false;
                        }
                    }
                }
            }

            return new CommandResult(command, success);
        }
    }
}