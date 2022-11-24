using System.IO.Compression;
using System.Xml;
using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NuGet.Versioning;

const string solutionFile = "TeamCity.CSharpInteractive.sln";
const string packageId = "TeamCity.CSharpInteractive";
const string toolPackageId = "TeamCity.csi";
const string templatesPackageId = "TeamCity.CSharpInteractive.Templates";
var frameworks = new[] {"net6.0", "net7.0"};

var currentDir = Environment.CurrentDirectory;
if (!File.Exists(solutionFile))
{
    Error($"Cannot find the solution \"{solutionFile}\". Current directory is \"{currentDir}\".");
    return 1;
}

var configuration = Property.Get("configuration", "Release");
var apiKey = Property.Get("apiKey", "");
var integrationTests = bool.Parse(Property.Get("integrationTests", Tools.UnderTeamCity.ToString()));
var defaultVersion = NuGetVersion.Parse(Property.Get("version", "1.0.0-dev", Tools.UnderTeamCity));
var outputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive", "bin", configuration);
var templateOutputDir = Path.Combine(currentDir, "TeamCity.CSharpInteractive.Templates", "bin", configuration);

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
    Warning("The docker Linux container is not available. Integration tests are skipped.");
}

var packageVersion = new[]
{
    Version.GetNext(new NuGetRestoreSettings(toolPackageId).WithPackageType(NuGetPackageType.Tool), defaultVersion),
    Version.GetNext(new NuGetRestoreSettings(packageId), defaultVersion)
}.Max()!;

var templatePackageVersion = Version.GetNext(new NuGetRestoreSettings(templatesPackageId), new NuGetVersion(1, 0, 0));

var packages = new[]
{
    new PackageInfo(
        packageId,
        Path.Combine("TeamCity.CSharpInteractive", "TeamCity.CSharpInteractive.csproj"),
        Path.Combine(outputDir, "TeamCity.CSharpInteractive", $"{packageId}.{packageVersion.ToString()}.nupkg"),
        packageVersion,
        true),
    
    new PackageInfo(
        toolPackageId,
        Path.Combine("TeamCity.CSharpInteractive", "TeamCity.CSharpInteractive.Tool.csproj"),
        Path.Combine(outputDir, "TeamCity.CSharpInteractive.Tool", $"{toolPackageId}.{packageVersion.ToString()}.nupkg"),
        packageVersion,
        true),
    
    new PackageInfo(
        templatesPackageId,
        Path.Combine("TeamCity.CSharpInteractive.Templates", "TeamCity.CSharpInteractive.Templates.csproj"),
        Path.Combine(templateOutputDir, $"{templatesPackageId}.{templatePackageVersion.ToString()}.nupkg"),
        templatePackageVersion,
        false)
};

Assertion.Succeed(
    new DotNetToolRestore()
        .Run(),
    "Restore tools"
);

Assertion.Succeed(
    new DotNetClean()
        .WithProject(solutionFile)
        .WithVerbosity(DotNetVerbosity.Quiet)
        .WithConfiguration(configuration)
        .Build()
);

foreach (var package in packages)
{
    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages", package.Id, package.Version.ToString());
    if (Directory.Exists(path))
    {
        Directory.Delete(path, true);
    }
}

var buildProps = new[] {("version", packageVersion.ToString())};
Assertion.Succeed(
    new DotNetClean()
        .WithProject(solutionFile)
        .WithConfiguration(configuration)
        .Build());

Assertion.Succeed(
    new DotNetBuild()
        .WithProject(solutionFile)
        .WithConfiguration(configuration)
        .WithProps(buildProps)
        .Build());

var reportDir = Path.Combine(currentDir, ".reports");
if (Directory.Exists(reportDir))
{
    Directory.Delete(reportDir, true);
}

var test = 
    new DotNetTest()
        .WithProject(solutionFile)
        .WithConfiguration(configuration)
        .WithNoBuild(true)
        .WithProps(buildProps)
        .WithFilter("Integration!=true&Docker!=true");

var dotCoverSnapshot = Path.Combine(reportDir, "dotCover.dcvr");
Assertion.Succeed(
    test
        .Customize(cmd => 
            cmd.WithArgs("dotcover")
                .AddArgs(cmd.Args)
                .AddArgs(
                    $"--dcOutput={dotCoverSnapshot}",
                    "--dcFilters=+:module=TeamCity.CSharpInteractive.HostApi;+:module=dotnet-csi",
                    "--dcAttributeFilters=System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage"))
        .Build());

var dotCoverReportDir = Path.Combine(reportDir, "html");
var dotCoverReportHtml = Path.Combine(dotCoverReportDir, "index.html");
var dotCoverReportXml = Path.Combine(reportDir, "dotCover.xml");
Assertion.Succeed(new DotNetCustom("dotCover", "report", $"--source={dotCoverSnapshot}", $"--output={dotCoverReportHtml};{dotCoverReportXml}", "--reportType=HTML,TeamCityXml").Run(), "Generating the code coverage reports");
var dotCoverReportDoc = new XmlDocument();
dotCoverReportDoc.Load(dotCoverReportXml);
var coveragePercentageValue = dotCoverReportDoc.SelectNodes("Root")?.Item(0)?.Attributes?["CoveragePercent"]?.Value;
if (int.TryParse(coveragePercentageValue, out var coveragePercentage))
{
    switch (coveragePercentage)
    {
        case < 80:
            Error($"The coverage percentage {coveragePercentage} is too low. See {dotCoverReportHtml} for details.");
            Assertion.Exit();
            break;

        case < 85:
            Warning($"The coverage percentage {coveragePercentage} is too low. See {dotCoverReportHtml} for details.");
            break;
    }
}
else
{
    Warning("Coverage percentage was not found.");
}

var dotCoverReportZip = Path.Combine(reportDir, "dotCover.zip");
ZipFile.CreateFromDirectory(dotCoverReportDir, dotCoverReportZip);
var teamCityWriter = GetService<ITeamCityWriter>();
teamCityWriter.PublishArtifact($"{dotCoverReportZip} => .");

foreach (var package in packages)
{
    var packageOutput = Path.GetDirectoryName(package.Package);
    if (Directory.Exists(packageOutput))
    {
        Directory.Delete(packageOutput, true);
    }

    Assertion.Succeed(new DotNetPack()
        .WithConfiguration(configuration)
        .WithProps(("version", package.Version.ToString()))
        .WithProject(package.Project)
        .Build());
}

var uninstallTool = new DotNetCustom("tool", "uninstall", toolPackageId, "-g")
    .WithShortName("Uninstalling tool");

if (uninstallTool.Run(output => WriteLine(output.Line)) != 0)
{
    Warning($"{uninstallTool} failed.");
}

var installTool = new DotNetCustom("tool", "install", toolPackageId, "-g", "--version", packageVersion.ToString(), "--add-source", Path.Combine(outputDir, "TeamCity.CSharpInteractive.Tool"))
    .WithShortName("Installing tool");

if (installTool.Run(output => WriteLine(output.Line)) != 0)
{
    Warning($"{installTool} failed.");
}

Assertion.Succeed(new DotNetCustom("csi", "/?").Run(), "Checking tool");

var uninstallTemplates = new DotNetCustom("new", "-u", templatesPackageId)
    .WithShortName("Uninstalling template");

if (uninstallTemplates.Run(output => WriteLine(output.Line)) != 0)
{
    Warning($"{uninstallTemplates} failed.");
}

var installTemplates = new DotNetCustom("new", "-i", $"{templatesPackageId}::{templatePackageVersion.ToString()}", "--nuget-source", templateOutputDir)
    .WithShortName("Installing template");

Assertion.Succeed(installTemplates.Run(), installTemplates.ShortName);

foreach (var framework in frameworks)
{
    var buildProjectDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()[..8]);
    Directory.CreateDirectory(buildProjectDir);
    var sampleProjectName = $"sample project for {framework}";
    try
    {
        var sampleProjectDir = Path.Combine("Samples", "DemoProject", "MySampleLib", "MySampleLib.Tests");
        Assertion.Succeed(new DotNetCustom("new", "build", $"--package-version={packageVersion}", "-T", framework, "--no-restore").WithWorkingDirectory(buildProjectDir).Run(), $"Creating a new {sampleProjectName}");
        Assertion.Succeed(new DotNetBuild().WithProject(buildProjectDir).AddSources(Path.Combine(outputDir, "TeamCity.CSharpInteractive")).WithShortName($"Building the {sampleProjectName}").Build());
        Assertion.Succeed(new DotNetRun().WithProject(buildProjectDir).WithNoBuild(true).WithWorkingDirectory(sampleProjectDir).Run(), $"Running a build for the {sampleProjectName}");
        Assertion.Succeed(new DotNetCustom("csi", Path.Combine(buildProjectDir, "Program.csx")).WithWorkingDirectory(sampleProjectDir).Run(), $"Running a build as a C# script for the {sampleProjectName}");
    }
    finally
    {
        Directory.Delete(buildProjectDir, true);
    }
}

Info("Publishing artifacts.");
foreach (var package in packages)
{
    if (!File.Exists(package.Package))
    {
        Error($"NuGet package {package.Package} does not exist.");
        return 1;
    }

    teamCityWriter.PublishArtifact($"{package.Package} => .");
}

if (!string.IsNullOrWhiteSpace(apiKey) && packageVersion.Release != "dev" && templatePackageVersion.Release != "dev")
{
    var push = new DotNetNuGetPush().WithApiKey(apiKey).WithSources("https://api.nuget.org/v3/index.json");
    foreach (var package in packages.Where(i => i.Publish))
    {
        Assertion.Succeed(push.WithPackage(package.Package).Run(), $"Pushing {Path.GetFileName(package.Package)}");
    }
}
else
{
    Info("Pushing NuGet packages were skipped.");
}

if (integrationTests || dockerLinuxTests)
{
    var logicOp = integrationTests && dockerLinuxTests ? "|" : "&";
    var filter = $"Integration={integrationTests}{logicOp}Docker={dockerLinuxTests}";
    Assertion.Succeed(test.WithFilter(filter).Build());
}

WriteLine("To use the csi tool:", Color.Highlighted);
WriteLine("    dotnet csi /?", Color.Highlighted);
WriteLine("To create a build project from the template:", Color.Highlighted);
WriteLine($"    dotnet new build --package-version={packageVersion}", Color.Highlighted);
WriteLine($"Tool and package version: {packageVersion}", Color.Highlighted);
WriteLine($"Template version: {templatePackageVersion}", Color.Highlighted);
WriteLine($"The coverage percentage: {coveragePercentage}", Color.Highlighted);
return 0;

record PackageInfo(string Id, string Project, string Package, NuGetVersion Version, bool Publish);