using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NuGet.Versioning;
using HostApi;

const string packageId = "TeamCity.CSharpInteractive";
const string toolPackageId = "TeamCity.csi";
const string templatesPackageId = "TeamCity.CSharpInteractive.Templates";

if (!Props.TryGetValue("configuration", out var configuration)) configuration = "Release";
if (!Props.TryGetValue("integrationTests", out var integrationStr) || !bool.TryParse(integrationStr, out var integrationTests)) integrationTests = false;

var currentDir = Environment.CurrentDirectory;
const string solutionFile = "TeamCity.CSharpInteractive.sln"; 
if (!File.Exists(solutionFile))
{
    Error($"Cannot find the solution \"{solutionFile}\". Current directory is \"{currentDir}\".");
    return 1;
}

var outputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive", "bin", configuration);
var templatesOutputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive.Templates", "bin", configuration);

// Package version
NuGetVersion GetNextVersion(NuGetRestore settings) =>
    GetService<INuGet>()
        .Restore(settings.WithHideWarningsAndErrors(true))
        .Where(i => i.Name == settings.PackageId)
        .Select(i => i.NuGetVersion)
        .Select(i => new NuGetVersion(i.Major, i.Minor, i.Patch + 1))
        .DefaultIfEmpty(string.IsNullOrWhiteSpace(Props["version"]) ? new NuGetVersion(1, 0, 0, "dev") : new NuGetVersion(Props["version"]))
        .Max()!;

var toolRestoreSettings = new NuGetRestore(toolPackageId)
    .WithPackageType(NuGetPackageType.Tool);

var packageRestoreSettings = new NuGetRestore(packageId)
    .WithPackageType(NuGetPackageType.Tool);

var nextToolAndPackageVersion = new []{ GetNextVersion(toolRestoreSettings),  GetNextVersion(packageRestoreSettings) }.Max()!;

WriteLine($"Tool and package version: {nextToolAndPackageVersion}");

var templateRestoreSettings = new NuGetRestore(templatesPackageId)
    .WithPackageType(NuGetPackageType.Package);

var nextTemplateVersion = GetNextVersion(templateRestoreSettings);

WriteLine($"Template version: {nextTemplateVersion}");

var buildProps = new[] { ("version", nextToolAndPackageVersion.ToString()) };

var runner = GetService<IBuildRunner>();

var clean = new DotNetClean()
    .WithProject(solutionFile)
    .WithConfiguration(configuration);

if(runner.Run(clean).ExitCode != 0) return 1;

var build = new DotNetBuild()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);

if(runner.Run(build).ExitCode != 0) return 1;

var test = new DotNetTest()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithNoBuild(true)
    .WithProps(buildProps);

if (!integrationTests)
{
    test = test.WithFilter("Integration!=true");
}

var result = runner.Run(test);
if (result.ExitCode != 0 || result.Summary.FailedTests != 0)
{
    foreach (var failedTest in result.Tests.Where(i => i.State == TestState.Failed))
    {
        Error(failedTest.ToString());
    }

    return 1;
}

var pack = new DotNetPack()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);
    
if (runner.Run(pack).ExitCode != 0) return 1;

var uninstallTool = new DotNetCustom("tool", "uninstall", toolPackageId, "-g");
if (runner.Run(uninstallTool).ExitCode != 0) return 1;

var installTool = new DotNetCustom("tool", "install", toolPackageId, "-g", "--version", nextToolAndPackageVersion.ToString(), "--add-source", outputDir);
if (runner.Run(installTool).ExitCode != 0) return 1;

var runTool = new DotNetCustom( "csi", "/?");
if (runner.Run(runTool).ExitCode != 0) return 1;

var uninstallTemplates = new DotNetCustom("new", "-u", templatesPackageId)
    .WithWorkingDirectory(templatesOutputDir);
if (runner.Run(uninstallTemplates).ExitCode != 0) return 1;

var installTemplates = new DotNetCustom("new", "-i", $"{templatesPackageId}::{nextTemplateVersion.ToString()}", "--nuget-source", templatesOutputDir)
    .WithWorkingDirectory(templatesOutputDir);
if (runner.Run(installTemplates).ExitCode != 0) return 1;

var runTemplates = new DotNetCustom("new",  "build",  "--help");
if (runner.Run(runTemplates).ExitCode != 0) return 1;

Info("Publishing artifacts.");
var teamCityWriter = GetService<ITeamCityWriter>();

var nugetPackages = new[]
{
    Path.Combine(outputDir, $"{toolPackageId}.{nextToolAndPackageVersion.ToString()}.nupkg"),
    Path.Combine(outputDir, $"{packageId}.{nextToolAndPackageVersion.ToString()}.nupkg"),
    Path.Combine(templatesOutputDir, $"{templatesPackageId}.{nextTemplateVersion.ToString()}.nupkg")
};

foreach (var nugetPackage in nugetPackages)
{
    if (!File.Exists(nugetPackage))
    {
        Error($"NuGet package {nugetPackage} does not exist.");
        return 1;
    }

    teamCityWriter.PublishArtifact($"{nugetPackage} => .");
}

return 0;