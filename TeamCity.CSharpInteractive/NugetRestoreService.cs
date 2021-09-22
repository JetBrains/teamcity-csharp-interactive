// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Build.Tasks;
    using NuGet.Versioning;

    [ExcludeFromCodeCoverage]
    internal class NugetRestoreService : INugetRestoreService, ISettingSetter<NuGetRestoreSetting>
    {
        private const string Project = "restore";
        private readonly ILog<NugetRestoreService> _log;
        private readonly IBuildEngine _buildEngine;
        private readonly IUniqueNameGenerator _uniqueNameGenerator;
        private readonly IEnvironment _environment;
        private readonly IDotnetEnvironment _dotnetEnvironment;
        private readonly ITargetFrameworkMonikerParser _targetFrameworkMonikerParser;
        private readonly ISettings _settings;

        private bool _restoreDisableParallel;
        private bool _restoreIgnoreFailedSources;
        private bool _hideWarningsAndErrors;
        private bool _restoreNoCache;

        public NugetRestoreService(
            ILog<NugetRestoreService> log,
            IBuildEngine buildEngine,
            IUniqueNameGenerator uniqueNameGenerator,
            IEnvironment environment,
            IDotnetEnvironment dotnetEnvironment,
            ITargetFrameworkMonikerParser targetFrameworkMonikerParser,
            ISettings settings)
        {
            _log = log;
            _buildEngine = buildEngine;
            _uniqueNameGenerator = uniqueNameGenerator;
            _environment = environment;
            _dotnetEnvironment = dotnetEnvironment;
            _targetFrameworkMonikerParser = targetFrameworkMonikerParser;
            _settings = settings;
            SetSetting(NuGetRestoreSetting.Default);
        }

        public bool TryRestore(
            string packageId,
            VersionRange? versionRange,
            string? targetFrameworkMoniker,
            IEnumerable<string> sources,
            IEnumerable<string> fallbackFolders,
            string packagesPath,
            out string projectAssetsJson)
        {
            var tempDirectory = _environment.GetPath(SpecialFolder.Temp);
            var outputPath = Path.Combine(tempDirectory, _uniqueNameGenerator.Generate());
            var tfm = targetFrameworkMoniker ?? _dotnetEnvironment.TargetFrameworkMoniker;
            targetFrameworkMoniker = _targetFrameworkMonikerParser.Parse(tfm);
            _log.Trace($"Restore nuget package {packageId} {versionRange} to \"{outputPath}\" and \"{packagesPath}\".");
            var restoreGraphItems = new[]
            {
                CreateTaskItem("RestoreSpec"),

                // { "ConfigFilePaths", @"C:\Users\Nikol\AppData\Roaming\NuGet\NuGet.Config;C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.FallbackLocation.config;C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config;C:\Program Files (x86)\NuGet\Config\Xamarin.Offline.config" }
                CreateTaskItem(
                    "ProjectSpec",
                    ("ProjectName", Project),
                    ("ProjectStyle", "PackageReference"),
                    ("Sources", string.Join(";", sources)),
                    ("FallbackFolders", string.Join(";", fallbackFolders)),
                    ("OutputPath", outputPath),
                    ("PackagesPath", packagesPath)),

                CreateTaskItem(
                    "Dependency",
                    ("TargetFrameworks", tfm),
                    ("Id", packageId),
                    ("VersionRange", versionRange?.ToString())),

                CreateTaskItem(
                    "TargetFrameworkInformation",
                    ("TargetFramework", tfm),
                    ("TargetFrameworkMoniker", targetFrameworkMoniker))
            };

            projectAssetsJson = Path.Combine(outputPath, "project.assets.json");
            return new RestoreTask
            {
                RestoreDisableParallel = _restoreDisableParallel,
                RestoreIgnoreFailedSources = _restoreIgnoreFailedSources,
                HideWarningsAndErrors = _hideWarningsAndErrors,
                RestoreNoCache = _restoreNoCache,
                RestoreForceEvaluate = true,
                RestorePackagesConfig = false,
                RestoreRecursive = false,
                RestoreForce = true,
                Interactive = _settings.InteractionMode == InteractionMode.Interactive,
                RestoreGraphItems = restoreGraphItems,
                BuildEngine = _buildEngine
            }.Execute();
        }

        private static ITaskItem CreateTaskItem(string type, params (string key, string? value)[] properties)
        {
            const string projectFile = Project + ".csproj";
            var taskItem = new TaskItem(type);
            taskItem.SetMetadata("ProjectUniqueName", projectFile);
            taskItem.SetMetadata("MSBuildSourceProjectFile", projectFile);
            taskItem.SetMetadata("ProjectPath", projectFile);
            taskItem.SetMetadata("Type", type);
            foreach (var (key, value) in properties.Where(i => i.value != null))
            {
                taskItem.SetMetadata(key, value);
            }
            
            return taskItem;
        }

        public NuGetRestoreSetting? SetSetting(NuGetRestoreSetting value)
        {
            NuGetRestoreSetting prevVal = default;
            switch (value)
            {
                case NuGetRestoreSetting.Default:
                    _restoreDisableParallel = false;
                    _restoreIgnoreFailedSources = false;
                    _hideWarningsAndErrors = false;
                    _restoreNoCache = false;
                    break;
                
                case NuGetRestoreSetting.Parallel:
                    prevVal = _restoreDisableParallel ? NuGetRestoreSetting.Parallel : NuGetRestoreSetting.NonParallel; 
                    _restoreDisableParallel = true;
                    break;

                case NuGetRestoreSetting.NonParallel:
                    prevVal = _restoreDisableParallel ? NuGetRestoreSetting.Parallel : NuGetRestoreSetting.NonParallel;
                    _restoreDisableParallel = false;
                    break;

                case NuGetRestoreSetting.IgnoreFailedSources:
                    prevVal = _restoreIgnoreFailedSources ? NuGetRestoreSetting.IgnoreFailedSources : NuGetRestoreSetting.ConsiderFailedSources;
                    _restoreIgnoreFailedSources = true;
                    break;

                case NuGetRestoreSetting.ConsiderFailedSources:
                    prevVal = _restoreIgnoreFailedSources ? NuGetRestoreSetting.IgnoreFailedSources : NuGetRestoreSetting.ConsiderFailedSources;
                    _restoreIgnoreFailedSources = false;
                    break;

                case NuGetRestoreSetting.HideWarningsAndErrors:
                    prevVal = _hideWarningsAndErrors ? NuGetRestoreSetting.HideWarningsAndErrors : NuGetRestoreSetting.ShowWarningsAndErrors;
                    _hideWarningsAndErrors = true;
                    break;

                case NuGetRestoreSetting.ShowWarningsAndErrors:
                    prevVal = _hideWarningsAndErrors ? NuGetRestoreSetting.HideWarningsAndErrors : NuGetRestoreSetting.ShowWarningsAndErrors;
                    _hideWarningsAndErrors = false;
                    break;

                case NuGetRestoreSetting.NoCache:
                    prevVal = _restoreNoCache ? NuGetRestoreSetting.NoCache : NuGetRestoreSetting.WithCache;
                    _restoreNoCache = true;
                    break;

                case NuGetRestoreSetting.WithCache:
                    prevVal = _restoreNoCache ? NuGetRestoreSetting.NoCache : NuGetRestoreSetting.WithCache;
                    _restoreNoCache = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            return prevVal;
        }
    }
}