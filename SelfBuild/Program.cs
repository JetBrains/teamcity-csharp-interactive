using NuGet.Versioning;
using Script.DotNet;
using Script.NuGet;

var currentDir = Environment.CurrentDirectory;
const string solutionFile = "TeamCity.CSharpInteractive.sln"; 
if (!File.Exists(solutionFile))
{
    Error($"Cannot find the solution \"{solutionFile}\". Current directory is \"{currentDir}\".");
    return 1;
}

// Package version
NuGetVersion GetNextVersion(RestoreSettings settings) =>
    GetService<INuGet>()
        .Restore(settings.WithHideWarningsAndErrors(true))
        .Where(i => i.Name == settings.PackageId)
        .Select(i => i.NuGetVersion)
        .Select(i => new NuGetVersion(i.Major, i.Minor, i.Patch + 1))
        .DefaultIfEmpty(string.IsNullOrWhiteSpace(Props["version"]) ? new NuGetVersion(1, 0, 0, "dev") : new NuGetVersion(Props["version"]))
        .Max()!;

var toolRestoreSettings = new RestoreSettings("TeamCity.csi")
    .WithPackageType(PackageType.Tool)
    .WithVersionRange(VersionRange.All);

var nextToolVersion = GetNextVersion(toolRestoreSettings);
WriteLine($"Version {nextToolVersion}");

if (!Props.TryGetValue("configuration", out var configuration)) configuration = "Release";

var buildProps = new[] { ("version", nextToolVersion.ToString()) };

var runner = GetService<IBuildRunner>();

var clean = new Clean()
    .WithProject(solutionFile)
    .WithConfiguration(configuration);

if(runner.Run(clean).ExitCode != 0) return 1;

var build = new Build()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);

if(runner.Run(build).ExitCode != 0) return 1;

var test = new Test()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithNoBuild(true)
    .WithFilter("Integration!=true")
    .WithProps(buildProps);

var result = runner.Run(test);
if (result.ExitCode != 0 || result.Summary.FailedTests != 0)
{
    foreach (var failedTest in result.Tests.Where(i => i.State == TestState.Failed))
    {
        Error(failedTest.ToString());
    }

    return 1;
}

var pack = new Pack()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);
    
if (runner.Run(pack).ExitCode != 0) return 1;

var uninstallTool = new Custom("tool", "uninstall", "TeamCity.csi", "-g");
if (runner.Run(uninstallTool).ExitCode != 0) return 1;

var installTool = new Custom("tool", "install", "TeamCity.csi", "-g", "--version", nextToolVersion.ToString(), "--add-source", Path.Combine(currentDir, "TeamCity.CSharpInteractive", "bin", configuration));
if (runner.Run(installTool).ExitCode != 0) return 1;

var runTool = new Custom( "csi", "/?");
if (runner.Run(runTool).ExitCode != 0) return 1;

var templatePackageDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive.Templates", "bin", configuration);
var uninstallTemplates = new Custom("new", "-u", "TeamCity.CSharpInteractive.Templates")
    .WithWorkingDirectory(templatePackageDir);
if (runner.Run(uninstallTemplates).ExitCode != 0) return 1;

var installTemplates = new Custom("new", "-i", $"TeamCity.CSharpInteractive.Templates::{nextToolVersion.ToString()}", "--nuget-source", templatePackageDir)
    .WithWorkingDirectory(templatePackageDir);
if (runner.Run(installTemplates).ExitCode != 0) return 1;

var runTemplates = new Custom("new",  "build",  "--help");
if (runner.Run(runTemplates).ExitCode != 0) return 1;

return 0;