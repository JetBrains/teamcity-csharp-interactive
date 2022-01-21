// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using Script;
using Script.NuGet;

internal class AddNuGetReferenceCommandRunner: ICommandRunner
{
    private readonly ILog<AddNuGetReferenceCommandRunner> _log;
    private readonly INuGetEnvironment _nugetEnvironment;
    private readonly INuGetRestoreService _nugetRestoreService;
    private readonly INuGetAssetsReader _nugetAssetsReader;
    private readonly ICleaner _cleaner;
    private readonly IReferenceRegistry _referenceRegistry;

    public AddNuGetReferenceCommandRunner(
        ILog<AddNuGetReferenceCommandRunner> log,
        INuGetEnvironment nugetEnvironment,
        INuGetRestoreService nugetRestoreService,
        INuGetAssetsReader nugetAssetsReader,
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
        _log.Info(new[] { new Text($"Restoring package {packageName}.", Color.Highlighted) });
        var restoreResult = _nugetRestoreService.TryRestore(
            new RestoreSettings(
                addPackageReferenceCommand.PackageId,
                _nugetEnvironment.Sources,
                _nugetEnvironment.FallbackFolders,
                addPackageReferenceCommand.VersionRange,
                default,
                _nugetEnvironment.PackagesPath
            ),
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
            _log.Info(new Text("Assemblies referenced:", Color.Highlighted));
            foreach (var assembly in _nugetAssetsReader.ReadReferencingAssemblies(projectAssetsJson))
            {
                if (_referenceRegistry.TryRegisterAssembly(assembly.FilePath, out var description))
                {
                    _log.Info(Text.Tab, new Text(assembly.Name, Color.Highlighted));
                }
                else
                {
                    _log.Error(ErrorId.NuGet, $"Cannot add the reference \"{assembly.Name}\": {description}");
                    success = false;
                }
            }
        }

        return new CommandResult(command, success);
    }
}