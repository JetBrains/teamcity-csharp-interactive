using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NuGet.Versioning;

const string solutionFile = "TeamCity.CSharpInteractive.sln";
const string packageId = "TeamCity.CSharpInteractive";
const string toolPackageId = "TeamCity.csi";
const string templatesPackageId = "TeamCity.CSharpInteractive.Templates";

var currentDir = Environment.CurrentDirectory;
if (!File.Exists(solutionFile))
{
    Error($"Cannot find the solution \"{solutionFile}\". Current directory is \"{currentDir}\".");
    return 1;
}

var underTeamCity = Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != default;
var configuration = Property.Get("configuration", "Release");
var apiKey = Property.Get("apiKey", "");
var integrationTests = bool.Parse(Property.Get("integrationTests", underTeamCity.ToString()));
var defaultVersion = NuGetVersion.Parse(Property.Get("version", "1.0.0-dev", underTeamCity));
var outputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive", "bin", configuration);
var templatesOutputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive.Templates", "bin", configuration);

var dockerLinuxTests = false;
new DockerCustom("info").WithShortName("Defining a docker container type")
    .Run(output =>
    {
        WriteLine("    " + output.Line, Color.Details);
        if (output.Line.Contains("OSType: linux"))
        {
            dockerLinuxTests = true;
        }
    });

if (!dockerLinuxTests)
{
    Warning("The docker Linux container is not available.");
}

var nextToolAndPackageVersion = new[]
{
    Version.GetNext(new NuGetRestoreSettings(toolPackageId).WithPackageType(NuGetPackageType.Tool), defaultVersion),
    Version.GetNext(new NuGetRestoreSettings(packageId), defaultVersion)
}.Max()!;

var nextTemplateVersion = Version.GetNext(new NuGetRestoreSettings(templatesPackageId), defaultVersion);

WriteLine($"Tool and package version: {nextToolAndPackageVersion}", Color.Highlighted);
WriteLine($"Template version: {nextTemplateVersion}", Color.Highlighted);

Assertion.Succeed(
    new DotNetClean()
        .WithProject(solutionFile)
        .WithConfiguration(configuration)
        .Build()
);

var buildProps = new[] {("version", nextToolAndPackageVersion.ToString())};
Assertion.Succeed(
    new DotNetBuild()
        .WithProject(solutionFile)
        .WithConfiguration(configuration)
        .WithProps(buildProps)
        .Build());

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

Assertion.Succeed(test.Build());

Assertion.Succeed(
    new DotNetPack()
        .WithProject(solutionFile)
        .WithConfiguration(configuration)
        .WithProps(buildProps)
        .Build());

var uninstallTool = new DotNetCustom("tool", "uninstall", toolPackageId, "-g")
    .WithShortName("Uninstalling tool");

if (uninstallTool.Run(output => WriteLine(output.Line)) != 0)
{
    Warning($"{uninstallTool} failed.");
}

var installTool = new DotNetCustom("tool", "install", toolPackageId, "-g", "--version", nextToolAndPackageVersion.ToString(), "--add-source", Path.Combine(outputDir, "TeamCity.CSharpInteractive.Tool"))
    .WithShortName("Installing tool");

if (installTool.Run(output => WriteLine(output.Line)) != 0)
{
    Warning($"{installTool} failed.");
}

Assertion.Succeed(new DotNetCustom("csi", "/?").Run(), "Checking tool");

var uninstallTemplates = new DotNetCustom("new", "-u", templatesPackageId)
    .WithWorkingDirectory(templatesOutputDir)
    .WithShortName("Uninstalling template");

if (uninstallTemplates.Run(output => WriteLine(output.Line)) != 0)
{
    Warning($"{uninstallTemplates} failed.");
}

var installTemplates = new DotNetCustom("new", "-i", $"{templatesPackageId}::{nextTemplateVersion.ToString()}", "--nuget-source", templatesOutputDir)
    .WithWorkingDirectory(templatesOutputDir)
    .WithShortName("Installing template");

Assertion.Succeed(installTemplates.Run(), installTemplates.ShortName);

var buildProjectDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()[..8]);
Directory.CreateDirectory(buildProjectDir);
var sampleProjectDir = Path.Combine("Samples", "DemoProject", "MySampleLib", "MySampleLib.Tests");

Assertion.Succeed(new DotNetCustom("new", "build", $"--package-version={nextToolAndPackageVersion}").WithWorkingDirectory(buildProjectDir).Run(), "Creating new build project");
Assertion.Succeed(new DotNetBuild().WithProject(buildProjectDir).AddSources(Path.Combine(outputDir, "TeamCity.CSharpInteractive")).WithShortName("Building a build project").Build());
Assertion.Succeed(new DotNetRun().WithProject(buildProjectDir).WithNoBuild(true).WithWorkingDirectory(sampleProjectDir).Run(), "Running a build as a console application");
Assertion.Succeed(new CommandLine("dotnet", "csi", Path.Combine(buildProjectDir, "Program.csx")).WithWorkingDirectory(sampleProjectDir).Run(), "Running a build as a C# script");

WriteLine("To use the csi tool:", Color.Highlighted);
WriteLine("    dotnet csi /?", Color.Highlighted);
WriteLine("To create a build project from the template:", Color.Highlighted);
WriteLine($"    dotnet new build --package-version={nextToolAndPackageVersion}", Color.Highlighted);

Info("Publishing artifacts.");
var teamCityWriter = GetService<ITeamCityWriter>();

var nugetPackages = new[]
{
    Path.Combine(outputDir, "TeamCity.CSharpInteractive.Tool", $"{toolPackageId}.{nextToolAndPackageVersion.ToString()}.nupkg"),
    Path.Combine(outputDir, "TeamCity.CSharpInteractive", $"{packageId}.{nextToolAndPackageVersion.ToString()}.nupkg"),
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

if (!string.IsNullOrWhiteSpace(apiKey) && nextToolAndPackageVersion.Release != "dev" && nextTemplateVersion.Release != "dev")
{
    var push = new DotNetNuGetPush().WithApiKey(apiKey);
    foreach (var nugetPackage in nugetPackages)
    {
        Assertion.Succeed(push.WithPackage(nugetPackage).Run(), $"Pushing {Path.GetFileName(nugetPackage)}");
    }
}
else
{
    Info("Pushing NuGet packages were skipped.");
}

return 0;