// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Build.Tasks;
    using NuGet.Versioning;

    [ExcludeFromCodeCoverage]
    internal class NugetRestoreService : INugetRestoreService
    {
        private const string Project = "restore";
        private readonly ILog<NugetRestoreService> _log;
        private readonly IBuildEngine _buildEngine;
        private readonly IUniqueNameGenerator _uniqueNameGenerator;
        private readonly IEnvironment _environment;
        private readonly IDotnetEnvironment _dotnetEnvironment;
        private readonly ITargetFrameworkMonikerParser _targetFrameworkMonikerParser;

        public NugetRestoreService(
            ILog<NugetRestoreService> log,
            IBuildEngine buildEngine,
            IUniqueNameGenerator uniqueNameGenerator,
            IEnvironment environment,
            IDotnetEnvironment dotnetEnvironment,
            ITargetFrameworkMonikerParser targetFrameworkMonikerParser)
        {
            _log = log;
            _buildEngine = buildEngine;
            _uniqueNameGenerator = uniqueNameGenerator;
            _environment = environment;
            _dotnetEnvironment = dotnetEnvironment;
            _targetFrameworkMonikerParser = targetFrameworkMonikerParser;
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
                    ("ProjectName", Project ),
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
                RestoreDisableParallel = false,
                RestoreIgnoreFailedSources = false,
                HideWarningsAndErrors = false,
                RestoreForceEvaluate = false,
                RestorePackagesConfig = true,
                RestoreRecursive = true,
                Interactive = false,
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
    }
}