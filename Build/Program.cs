using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NuGet.Versioning;

const string packageId = "TeamCity.CSharpInteractive";
const string toolPackageId = "TeamCity.csi";
const string templatesPackageId = "TeamCity.CSharpInteractive.Templates";

var configuration = GetProperty("configuration", "Release");
var integrationTests = bool.Parse(GetProperty("integrationTests", "false"));
var defaultVersion = NuGetVersion.Parse(GetProperty("defaultVersion", "1.0.0-dev"));

var dockerLinuxTests = false;
var commandLineRunner = GetService<ICommandLineRunner>();
commandLineRunner.Run(new DockerCustom("info"), output =>
{
    if (output.Line.Contains("OSType: linux"))
    {
        dockerLinuxTests = true;
    }
});

if (!dockerLinuxTests)
{
    Warning("The docker Linux container is not available.");
}

var currentDir = Environment.CurrentDirectory;
const string solutionFile = "TeamCity.CSharpInteractive.sln";
if (!File.Exists(solutionFile))
{
    Error($"Cannot find the solution \"{solutionFile}\". Current directory is \"{currentDir}\".");
    return 1;
}

var outputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive", "bin", configuration);
var templatesOutputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive.Templates", "bin", configuration);

var nextToolAndPackageVersion = new[]
{
    GetNextVersion(new NuGetRestore(toolPackageId).WithPackageType(NuGetPackageType.Tool)) ?? defaultVersion,
    GetNextVersion(new NuGetRestore(packageId)) ?? defaultVersion
}.Max()!;

var nextTemplateVersion = GetNextVersion(new NuGetRestore(templatesPackageId)) ?? defaultVersion;

WriteLine($"Tool and package version: {nextToolAndPackageVersion}");
WriteLine($"Template version: {nextTemplateVersion}");

var runner = GetService<IBuildRunner>();

var clean = new DotNetClean()
    .WithProject(solutionFile)
    .WithConfiguration(configuration);

if (runner.Run(clean).ExitCode != 0)
{
    Error("Cleaning failed.");
    return 1;
}

var buildProps = new[] {("version", nextToolAndPackageVersion.ToString())};
var build = new DotNetBuild()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);

if (runner.Run(build).ExitCode != 0)
{
    Error("Building failed.");
    return 1;
}

var test = new DotNetTest()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithNoBuild(true)
    .WithProps(buildProps);

if (!integrationTests)
{
    test = test.WithFilter("Integration!=true");
    Warning("Integration tests were skipped.");
}

if (!dockerLinuxTests)
{
    test = test.WithFilter(string.Join('&', test.Filter, "Integration!=true"));
    Warning("Docker tests were skipped.");
}

var result = runner.Run(test);
if (result.ExitCode != 0 || result.Summary.FailedTests != 0)
{
    foreach (var failedTest in result.Tests.Where(i => i.State == TestState.Failed))
    {
        Error(failedTest.ToString());
    }

    Error("Testing failed.");
    return 1;
}

var pack = new DotNetPack()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);

if (runner.Run(pack).ExitCode != 0)
{
    Error("Packing failed.");
    return 1;
}

var uninstallTool = new DotNetCustom("tool", "uninstall", toolPackageId, "-g");
runner.Run(uninstallTool);

var installTool = new DotNetCustom("tool", "install", toolPackageId, "-g", "--version", nextToolAndPackageVersion.ToString(), "--add-source", outputDir);
if (runner.Run(installTool).ExitCode != 0)
{
    Error($"{installTool.ShortName} failed.");
    return 1;
}

var runTool = new DotNetCustom("csi", "/?");
if (runner.Run(runTool).ExitCode != 0)
{
    Error($"{runTool.ShortName} failed.");
    return 1;
}

var uninstallTemplates = new DotNetCustom("new", "-u", templatesPackageId)
    .WithWorkingDirectory(templatesOutputDir);
runner.Run(uninstallTemplates);

var installTemplates = new DotNetCustom("new", "-i", $"{templatesPackageId}::{nextTemplateVersion.ToString()}", "--nuget-source", templatesOutputDir)
    .WithWorkingDirectory(templatesOutputDir);
if (runner.Run(installTemplates).ExitCode != 0)
{
    Error($"{installTemplates.ShortName} failed.");
    return 1;
}

var runTemplates = new DotNetCustom("new", "build", "--help");
if (runner.Run(runTemplates).ExitCode != 0)
{
    Error($"{runTemplates.ShortName} failed.");
    return 1;
}

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

WriteLine("To use the csi tool:", Color.Highlighted);
WriteLine("    dotnet csi /?", Color.Highlighted);
WriteLine("To create a build project from the template:", Color.Highlighted);
WriteLine("    dotnet new build --package-version=1.0.0-dev", Color.Highlighted);

return 0;

string GetProperty(string name, string defaultProp)
{
    var prop = Props[name];
    if (!string.IsNullOrWhiteSpace(prop))
    {
        return prop;
    }

    Warning($"The property \"{name}\" was not defined, the default value \"{defaultProp}\" was used.");
    return defaultProp;
}

NuGetVersion? GetNextVersion(NuGetRestore settings) =>
    GetService<INuGet>()
        .Restore(settings.WithHideWarningsAndErrors(true))
        .Where(i => i.Name == settings.PackageId)
        .Select(i => i.NuGetVersion)
        .Select(i => new NuGetVersion(i.Major, i.Minor, i.Patch + 1))
        .Max();
