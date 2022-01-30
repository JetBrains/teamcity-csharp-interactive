// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using HostApi;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet.Build.Tasks;

[ExcludeFromCodeCoverage]
internal class NuGetRestoreService : INuGetRestoreService, ISettingSetter<NuGetRestoreSetting>
{
    private const string Project = "restore";
    private readonly ILog<NuGetRestoreService> _log;
    private readonly IBuildEngine _buildEngine;
    private readonly IUniqueNameGenerator _uniqueNameGenerator;
    private readonly IEnvironment _environment;
    private readonly IDotNetEnvironment _dotnetEnvironment;
    private readonly ITargetFrameworkMonikerParser _targetFrameworkMonikerParser;
    private readonly ISettings _settings;

    private bool _restoreDisableParallel;
    private bool _restoreIgnoreFailedSources;
    private bool _hideWarningsAndErrors;
    private bool _restoreNoCache;

    public NuGetRestoreService(
        ILog<NuGetRestoreService> log,
        IBuildEngine buildEngine,
        IUniqueNameGenerator uniqueNameGenerator,
        IEnvironment environment,
        IDotNetEnvironment dotnetEnvironment,
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

    public bool TryRestore(NuGetRestore settings, out string projectAssetsJson)
    {
        var tempDirectory = _environment.GetPath(SpecialFolder.Temp);
        var outputPath = Path.Combine(tempDirectory, _uniqueNameGenerator.Generate());
        var targetFrameworkMoniker = settings.TargetFrameworkMoniker;
        var tfm = targetFrameworkMoniker ?? _dotnetEnvironment.TargetFrameworkMoniker;
        targetFrameworkMoniker = _targetFrameworkMonikerParser.Parse(tfm);
        var projectStyle = settings.PackageType switch
        {
            NuGetPackageType.Tool => "DotNetToolReference ",
            _ => "PackageReference"
        };

        _log.Trace(() => new[] {new Text($"Restore nuget package {settings.PackageId} {settings.VersionRange} to \"{outputPath}\" and \"{settings.PackagesPath}\".")});
        var restoreGraphItems = new[]
        {
            CreateTaskItem("RestoreSpec"),

            // { "ConfigFilePaths", @"C:\Users\Nikol\AppData\Roaming\NuGet\NuGet.Config;C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.FallbackLocation.config;C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config;C:\Program Files (x86)\NuGet\Config\Xamarin.Offline.config" }
            CreateTaskItem(
                "ProjectSpec",
                ("ProjectName", Project),
                ("ProjectStyle", projectStyle),
                ("Sources", string.Join(";", settings.Sources)),
                ("FallbackFolders", string.Join(";", settings.FallbackFolders)),
                ("OutputPath", outputPath),
                ("PackagesPath", settings.PackagesPath),
                ("ValidateRuntimeAssets", "false")),

            CreateTaskItem(
                "Dependency",
                ("TargetFrameworks", tfm),
                ("Id", settings.PackageId),
                ("VersionRange", settings.VersionRange?.ToString() ?? "*"),
                ("IncludeAssets", "All")),

            CreateTaskItem(
                "TargetFrameworkInformation",
                ("TargetFramework", tfm),
                ("TargetFrameworkMoniker", targetFrameworkMoniker))
        };

        projectAssetsJson = Path.Combine(outputPath, "project.assets.json");
        return new RestoreTask
        {
            RestoreDisableParallel = settings.DisableParallel ?? _restoreDisableParallel,
            RestoreIgnoreFailedSources = settings.IgnoreFailedSources ?? _restoreIgnoreFailedSources,
            HideWarningsAndErrors = settings.HideWarningsAndErrors ?? _hideWarningsAndErrors,
            RestoreNoCache = settings.NoCache ?? _restoreNoCache,
            RestoreForceEvaluate = false,
            RestorePackagesConfig = false,
            RestoreRecursive = true,
            RestoreForce = false,
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

    public NuGetRestoreSetting SetSetting(NuGetRestoreSetting value)
    {
        var prevVal = NuGetRestoreSetting.Default;
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