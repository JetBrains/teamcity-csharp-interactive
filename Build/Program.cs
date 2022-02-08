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
var integrationTests = bool.Parse(Property.Get("integrationTests", underTeamCity.ToString()));
var defaultVersion = NuGetVersion.Parse(Property.Get("defaultVersion", "1.0.0-dev", underTeamCity));
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
    GetNextVersion(new NuGetRestoreSettings(toolPackageId).WithPackageType(NuGetPackageType.Tool)) ?? defaultVersion,
    GetNextVersion(new NuGetRestoreSettings(packageId)) ?? defaultVersion
}.Max()!;

var nextTemplateVersion = GetNextVersion(new NuGetRestoreSettings(templatesPackageId)) ?? defaultVersion;

WriteLine($"Tool and package version: {nextToolAndPackageVersion}");
WriteLine($"Template version: {nextTemplateVersion}");

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

if (!underTeamCity)
{
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

    Assertion.Succeed(new DotNetCustom("new", "build", "--help").Run(), "Checking template");

    WriteLine("To use the csi tool:", Color.Highlighted);
    WriteLine("    dotnet csi /?", Color.Highlighted);
    WriteLine("To create a build project from the template:", Color.Highlighted);
    WriteLine($"    dotnet new build --package-version={nextToolAndPackageVersion}", Color.Highlighted);
}
else
{
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
}

return 0;

NuGetVersion? GetNextVersion(NuGetRestoreSettings settings)
{
    var floatRange = defaultVersion.Release != string.Empty
        ? new FloatRange(NuGetVersionFloatBehavior.Prerelease, defaultVersion)
        : new FloatRange(NuGetVersionFloatBehavior.Minor, defaultVersion);

    return GetService<INuGet>()
        .Restore(settings.WithHideWarningsAndErrors(true).WithVersionRange(new VersionRange(defaultVersion, floatRange)))
        .Where(i => i.Name == settings.PackageId)
        .Select(i => i.NuGetVersion)
        .Select(i => defaultVersion.Release != string.Empty
            ? new NuGetVersion(i.Major, i.Minor, i.Patch, defaultVersion.Release)
            : new NuGetVersion(i.Major, i.Minor, i.Patch + 1))
        .Max();
}