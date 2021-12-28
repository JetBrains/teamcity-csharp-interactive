#l diagnostic
using NuGet;
using Dotnet;
using Docker;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

var currentDirectory = Environment.CurrentDirectory;

// Target configuration
var configuration = string.IsNullOrEmpty(Props["configuration"]) ? "Release" : Props["configuration"];

// Test attempts for flaky tests
if (!int.TryParse(Props["attempts"], out var testAttempts) || testAttempts < 1) testAttempts = 3;

// Target directory
var outputDir = Path.Combine(currentDirectory, "bin");

// Required .NET SDK version
var requiredSdkVersion = new Version(6, 0);

// NuGet package id
const string packageId = "MySampleLib";

// Package version
var packageVersion = Props["version"];
if (string.IsNullOrEmpty(packageVersion))
{
    Info("Evaluate next NuGet package version.");
    packageVersion = 
        GetService<INuGet>()
        .Restore(packageId, "*")
        .Where(i => i.Name == packageId)
        .Select(i => i.Version)
        .Select(i => new Version(i.Major, i.Minor, i.Build + 1))
        .DefaultIfEmpty(new Version(1, 0, 0))
        .Max()!
        .ToString();
}

Trace($"Package version is: {packageVersion}.");

var commonProps = new[]{ 
    ("Version", packageVersion),
    ("ContinuousIntegrationBuild", "true")
};

var build = GetService<IBuild>();

Info($"Check the required .NET SDK version {requiredSdkVersion}.");
var sdkVersion = new Version();
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

var cleanResult = build.Run(new Clean());
if (!cleanResult.Success)
{
    Error(cleanResult);
    return;
}

var msbuildResult = build.Run(
    new MSBuild()
        .WithShortName("Rebuild solution")
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
    .Select((test, index) => build.Run(test.WithShortName($"Test - attempt {index + 1}"), _ => {}))
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

var buildResult = build.Run(
    new Build()
        .WithShortName($"Build a {configuration} version.")
        .WithConfiguration(configuration)
        .WithOutput(outputDir)
        .WithVerbosity(Verbosity.Normal)
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

var testInContainerTask = build.RunAsync(dockerTestCommand);
var vsTestTask = build.RunAsync(new VSTest().WithTestFileNames(Path.Combine(outputDir, "MySampleLib.Tests.dll")));
Task.WaitAll(testInContainerTask, vsTestTask);
WriteLine($"Parallel tests completed.");

if (!testInContainerTask.Result.Success)
{
    Error(testInContainerTask.Result);
    return;
}

if (!vsTestTask.Result.Success)
{
    Error(vsTestTask.Result);
    return;
}

var packResult = build.Run(
    new Pack()
        .WithShortName($"Pack {configuration} version.")
        .WithConfiguration(configuration)
        .WithOutput(outputDir)
        .WithIncludeSymbols(true)
        .WithIncludeSource(true)
        .WithVerbosity(Verbosity.Normal)
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
    var flakyTestsFile = Path.Combine(outputDir, "FlakyTests.txt");
    File.WriteAllLines(flakyTestsFile, flakyTests);
    teamCityWriter.PublishArtifact($"{flakyTestsFile} => .");
}

var artifacts = 
    from packageExtension in new [] { "nupkg", "symbols.nupkg" }
    let path = Path.Combine(outputDir, $"{packageId}.{packageVersion}.{packageExtension}")
    select $"{path} => .";

foreach (var artifact in artifacts)
{
    teamCityWriter.PublishArtifact(artifact);
}
