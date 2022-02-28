// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;
using NuGet.Versioning;

internal class NuGetReferenceResolver : INuGetReferenceResolver
{
    private readonly ILog<NuGetReferenceResolver> _log;
    private readonly INuGetEnvironment _nugetEnvironment;
    private readonly INuGetRestoreService _nugetRestoreService;
    private readonly INuGetAssetsReader _nugetAssetsReader;
    private readonly ICleaner _cleaner;

    public NuGetReferenceResolver(
        ILog<NuGetReferenceResolver> log,
        INuGetEnvironment nugetEnvironment,
        INuGetRestoreService nugetRestoreService,
        INuGetAssetsReader nugetAssetsReader,
        ICleaner cleaner)
    {
        _log = log;
        _nugetEnvironment = nugetEnvironment;
        _nugetRestoreService = nugetRestoreService;
        _nugetAssetsReader = nugetAssetsReader;
        _cleaner = cleaner;
    }

    public bool TryResolveAssemblies(string packageId, VersionRange? versionRange, out IReadOnlyCollection<ReferencingAssembly> assemblies)
    {
        var result = new List<ReferencingAssembly>();
        assemblies = result;
        var packageName = $"{packageId} {versionRange}".Trim();
        _log.Info(new[] {new Text($"Restoring package {packageName}.", Color.Highlighted)});
        var restoreResult = _nugetRestoreService.TryRestore(
            new NuGetRestoreSettings(
                packageId,
                _nugetEnvironment.Sources,
                _nugetEnvironment.FallbackFolders,
                versionRange,
                default,
                _nugetEnvironment.PackagesPath
            ),
            out var projectAssetsJson);

        if (!restoreResult)
        {
            return false;
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
                _log.Info(Text.Tab, new Text(assembly.Name, Color.Highlighted));
                result.Add(assembly);
            }
        }

        return true;
    }
}