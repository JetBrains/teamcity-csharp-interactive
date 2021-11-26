//#l diagnostic
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using NuGet;
using Cmd;
using Dotnet;
using Docker;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

var currentDirectory = System.Environment.CurrentDirectory;

// Package version
var packageVersion = Props["version"];

// Target configuration
string configuration = string.IsNullOrEmpty(Props["configuration"]) ? "Release" : Props["configuration"];

// Test attempts for flaky tests
if (!int.TryParse(Props["attempts"], out var testAttempts) || testAttempts < 1) testAttempts = 3;

// Target directory
string output = Path.Combine(currentDirectory, "bin");

// Required .NET SDK version
var requiredSdkVersion = new Version(6, 0);

// NuGet package id
var packageId = "MySampleLib";

if(string.IsNullOrEmpty(packageVersion))
{
    Info("Evaluate next NuGet package version.");
    packageVersion = 
        GetService<INuGet>()
        .Restore(packageId, "*")
        .Where(i => i.Name == packageId)
        .Select(i => i.Version)
        .Select(i => new Version(i.Major, i.Minor, i.Build + 1))
        .DefaultIfEmpty(new Version(1, 0, 0))
        .Max()
        .ToString();
}

Trace($"Package version is: {packageVersion}.");

var commonProps = new (string, string)[]{ 
    ("Version", packageVersion.ToString()),
    ("ContinuousIntegrationBuild", "true")
};

var build = GetService<IBuild>();

Info($"Check the required .NET SDK version {requiredSdkVersion}.");
Version sdkVersion;
if (build.Run(new Custom("--version"), output => Version.TryParse(output.Line, out sdkVersion)).Success)
{
    if (sdkVersion.Major != requiredSdkVersion.Major && sdkVersion.Minor != requiredSdkVersion.Minor)
    {
        Error($"Current SDK version is {sdkVersion}, but .NET SDK {requiredSdkVersion} is required.");
        return;
    }
}
else
{
    Error($"Cannot get an SDK version.");
    return;
}

Info("Clean.");
var cleanResult = build.Run(new Clean());
if (!cleanResult.Success)
{
    Error(cleanResult);
    return;
}

Info("Running MSBuild.");
var msbuildResult = build.Run(
    new MSBuild()
        .WithProject("MySampleLib.sln")
        .WithTarget("Rebuild")
        .WithRestore(true)
        .WithVerbosity(Verbosity.Normal)
        .AddProps(commonProps));

if (!msbuildResult.Success)
{
    Error(msbuildResult);
    return;
}

Info($"Running flaky tests with {testAttempts} attempts.");
var failedTests =
    Enumerable.Repeat(new Test().WithNoBuild(true).AddProps(commonProps), testAttempts)
    // Passing an output handler to avoid reporting to CI
    .Select(test => build.Run(test, output => {}))
    .TakeWhile(test => !test.Success)
    .ToList();

if (failedTests.Count == testAttempts)
{
    Error(failedTests.Last());
    return;
}

var flakyTests =
    failedTests
    .SelectMany(i => i.Tests)
    .Where(i => i.State == TestState.Failed)
    .Select(i => i.DisplayName)
    .Distinct()
    .OrderBy(i => i)
    .ToList();

Info($"Build a {configuration} version.");
var buildResult = build.Run(
    new Build()
        .WithConfiguration(configuration)
        .WithOutput(output)
        .AddProps(commonProps));

if (!buildResult.Success)
{
    Error(buildResult);
    return;
}

Info($"Running tests in Linux docker container and on the host in parallel.");
var testCommand = new Test().WithExecutablePath("dotnet").WithVerbosity(Verbosity.Normal);
var dockerImage = $"mcr.microsoft.com/dotnet/sdk:{requiredSdkVersion}";
var dockerTestCommand = new Docker.Run(testCommand, dockerImage)
    .WithPlatform("linux")
    .AddVolumes((currentDirectory, "/project"))
    .WithContainerWorkingDirectory("/project");

WriteLine($"Starting parallel tests.", Details);
var testInContainerTask = build.RunAsync(dockerTestCommand);
var vstestTask = build.RunAsync(new VSTest().WithTestFileNames(Path.Combine(output, "MySampleLib.Tests.dll")));
Task.WaitAll(testInContainerTask, vstestTask);
WriteLine($"Parallel tests completed.", Details);

if (!testInContainerTask.Result.Success)
{
    Error(testInContainerTask.Result);
    return;
}

if (!vstestTask.Result.Success)
{
    Error(vstestTask.Result);
    return;
}

Info($"Pack {configuration} version.");
var packResult = build.Run(
    new Pack()
        .WithConfiguration(configuration)
        .WithOutput(output)
        .WithIncludeSymbols(true)
        .WithIncludeSource(true)
        .AddProps(commonProps));

if (!packResult.Success)
{
    Error(packResult);
    return;
}

Info("Publish artifacts.");
var teamCityWriter = GetService<ITeamCityWriter>();

if (flakyTests.Any())
{
    Warning("Has flaky tests.");
    var flakyTestsFile = Path.Combine(output, "FlakyTests.txt");
    File.WriteAllLines(flakyTestsFile, flakyTests);
    teamCityWriter.PublishArtifact($"{flakyTestsFile} => .");
}

var artifacts = 
    from packageExtension in new [] { "nupkg", "symbols.nupkg" }
    let path = Path.Combine(output, $"{packageId}.{packageVersion}.{packageExtension}")
    select $"{path} => .";

foreach (var artifact in artifacts)
{
    teamCityWriter.PublishArtifact(artifact);
}