// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable NotAccessedPositionalProperty.Local
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using NuGet;
using NuGet.Common;
using NuGet.ProjectModel;
using NuGet.Versioning;

[ExcludeFromCodeCoverage]
internal class NuGetAssetsReader : INuGetAssetsReader
{
    private readonly ILog<NuGetAssetsReader> _log;
    private readonly ILogger _logger;
    private readonly IDotNetEnvironment _dotnetEnvironment;
    private readonly IFileSystem _fileSystem;

    public NuGetAssetsReader(
        ILog<NuGetAssetsReader> log,
        ILogger logger,
        IDotNetEnvironment dotnetEnvironment,
        IFileSystem fileSystem)
    {
        _log = log;
        _logger = logger;
        _dotnetEnvironment = dotnetEnvironment;
        _fileSystem = fileSystem;
    }

    public IEnumerable<NuGetPackage> ReadPackages(string packagesPath, string projectAssetsJson)
    {
        var lockFile = LockFileUtilities.GetLockFile(projectAssetsJson, _logger);
        // ReSharper disable once InvertIf
        if (lockFile == null)
        {
            _log.Warning($"Cannot process the lock file \"{projectAssetsJson}\".");
            return Enumerable.Empty<NuGetPackage>();
        }
            
        return lockFile.Libraries.Select(i => 
            new NuGetPackage(
                i.Name,
                i.Version.Version,
                i.Version,
                i.Type,
                Path.Combine(packagesPath, i.Path),
                i.Sha512,
                i.Files.ToList().AsReadOnly(),
                i.HasTools,
                i.IsServiceable));
    }

    public IEnumerable<ReferencingAssembly> ReadReferencingAssemblies(string projectAssetsJson)
    {
        var lockFile = LockFileUtilities.GetLockFile(projectAssetsJson, _logger);
        if (lockFile == null)
        {
            _log.Warning($"Cannot process the lock file \"{projectAssetsJson}\".");
            yield break;
        }
            
        var librariesDict = lockFile.Libraries.ToDictionary(i => new LibraryKey(i.Name, i.Version), i => i);

        var folders = lockFile.PackageFolders.Select(i => i.Path).ToHashSet();
        foreach (var target in lockFile.Targets)
        {
            _log.Trace(() => new []{new Text($"Processing target \"{target.Name}\".")});
            if (target.TargetFramework.DotNetFrameworkName != _dotnetEnvironment.TargetFrameworkMoniker)
            {
                _log.Trace(() => new []{new Text($"Skip processing of target \"{target.Name}\".")});
                continue;
            }

            foreach (var library in target.Libraries)
            {
                _log.Trace(() => new []{new Text($"Processing library \"{library.Name}\".")});
                if (!librariesDict.TryGetValue(new LibraryKey(library.Name, library.Version), out var lockFileLibrary))
                {
                    _log.Warning($"Cannot find the related library \"{library.Name}\", version {library.Version}.");
                    continue;
                }
                    
                foreach (var assembly in library.RuntimeAssemblies)
                {
                    _log.Trace(() => new []{new Text($"Processing assembly \"{assembly.Path}\".")});
                    var baseAssemblyPath = Path.Combine(lockFileLibrary.Path, assembly.Path);
                    _log.Trace(() => new []{new Text($"Base assembly path is \"{baseAssemblyPath}\".")});
                    foreach (var folder in folders)
                    {
                        var fullAssemblyPath = Path.Combine(folder, baseAssemblyPath);
                        _log.Trace(() => new []{new Text($"Full assembly path is \"{fullAssemblyPath}\".")});
                        if (!_fileSystem.IsFileExist(fullAssemblyPath))
                        {
                            _log.Trace(() => new []{new Text($"File \"{baseAssemblyPath}\" does not exist.")});
                            continue;
                        }

                        var ext = Path.GetExtension(fullAssemblyPath).ToLowerInvariant();
                        if (ext == ".dll")
                        {
                            _log.Trace(() => new []{new Text($"Add reference to \"{fullAssemblyPath}\".")});
                            yield return new ReferencingAssembly($"{Path.GetFileNameWithoutExtension(fullAssemblyPath)}", fullAssemblyPath);
                        }
                        else
                        {
                            _log.Trace(() => new []{new Text($"Skip file \"{fullAssemblyPath}\".")});
                        }

                        break;
                    }
                }
            }
        }
    }

    private readonly record struct LibraryKey(string Name, NuGetVersion Version);
}