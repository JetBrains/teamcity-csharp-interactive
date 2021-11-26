// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using NuGet;
    using NuGet.Common;
    using NuGet.ProjectModel;
    using NuGet.Versioning;

    [ExcludeFromCodeCoverage]
    internal class NugetAssetsReader : INugetAssetsReader
    {
        private readonly ILog<NugetAssetsReader> _log;
        private readonly ILogger _logger;
        private readonly IDotnetEnvironment _dotnetEnvironment;

        public NugetAssetsReader(
            ILog<NugetAssetsReader> log,
            ILogger logger,
            IDotnetEnvironment dotnetEnvironment)
        {
            _log = log;
            _logger = logger;
            _dotnetEnvironment = dotnetEnvironment;
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
            
            return lockFile.Libraries.Select(i => new NuGetPackage(i.Name, i.Version.Version, i.Type, Path.Combine(packagesPath, i.Path), i.Sha512));
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
                _log.Trace($"Processing target \"{target.Name}\".");
                if (target.TargetFramework.DotNetFrameworkName != _dotnetEnvironment.TargetFrameworkMoniker)
                {
                    _log.Trace($"Skip processing of target \"{target.Name}\".");
                    continue;
                }

                foreach (var library in target.Libraries)
                {
                    _log.Trace($"Processing library \"{library.Name}\".");
                    if (!librariesDict.TryGetValue(new LibraryKey(library.Name, library.Version), out var lockFileLibrary))
                    {
                        _log.Warning($"Cannot find the related library \"{library.Name}\", version {library.Version}.");
                        continue;
                    }
                    
                    foreach (var assembly in library.RuntimeAssemblies)
                    {
                        _log.Trace($"Processing assembly \"{assembly.Path}\".");
                        var baseAssemblyPath = Path.Combine(lockFileLibrary.Path, assembly.Path);
                        _log.Trace($"Base assembly path is \"{baseAssemblyPath}\".");
                        foreach (var folder in folders)
                        {
                            var fullAssemblyPath = Path.Combine(folder, baseAssemblyPath);
                            _log.Trace($"Full assembly path is \"{fullAssemblyPath}\".");
                            if (!File.Exists(fullAssemblyPath))
                            {
                                _log.Trace($"File \"{baseAssemblyPath}\" does not exist.");
                                continue;
                            }

                            var ext = Path.GetExtension(fullAssemblyPath).ToLowerInvariant();
                            if (ext == ".dll")
                            {
                                _log.Trace($"Add reference to \"{fullAssemblyPath}\".");
                                yield return new ReferencingAssembly($"{Path.GetFileNameWithoutExtension(fullAssemblyPath)}", fullAssemblyPath);
                            }
                            else
                            {
                                _log.Trace($"Skip file \"{fullAssemblyPath}\".");
                            }

                            break;
                        }
                    }
                }
            }
        }
        
        private readonly struct LibraryKey
        {
            private readonly string _name;
            private readonly NuGetVersion _version;

            public LibraryKey(string name, NuGetVersion version)
            {
                _name = name;
                _version = version;
            }

            public override bool Equals(object? obj) => obj is LibraryKey other && _name == other._name && _version.Equals(other._version);

            public override int GetHashCode() => HashCode.Combine(_name, _version);
        }
    }
}