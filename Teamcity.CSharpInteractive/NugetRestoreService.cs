// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
        private readonly IDotnetEnvironment _dotnetEnvironment;

        public NugetRestoreService(
            ILog<NugetRestoreService> log,
            IBuildEngine buildEngine,
            IDotnetEnvironment dotnetEnvironment)
        {
            _log = log;
            _buildEngine = buildEngine;
            _dotnetEnvironment = dotnetEnvironment;
        }

        public bool Restore(
            string packageId,
            VersionRange? versionRange,
            IEnumerable<string> sources,
            IEnumerable<string> fallbackFolders,
            string outputPath,
            string packagesPath)
        {
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
                    ("TargetFrameworks", _dotnetEnvironment.Tfm),
                    ("Id", packageId),
                    ("VersionRange", versionRange?.ToString())),

                CreateTaskItem(
                    "TargetFrameworkInformation",
                    ("TargetFramework", _dotnetEnvironment.Tfm),
                    ("TargetFrameworkMoniker", _dotnetEnvironment.TargetFrameworkMoniker))
            };

            return new RestoreTask
            {
                RestoreIgnoreFailedSources = true,
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