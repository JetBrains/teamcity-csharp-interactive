using NuGet;
using Dotnet;
using Docker;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NuGet.Versioning;

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
        .Select(i => i.NuGetVersion)
        .Select(i => new NuGetVersion(i.Major, i.Minor, i.Patch + 1))
        .DefaultIfEmpty(new NuGetVersion(1, 0, 0))
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
if (build.Run(new Custom("--version"), message => Version.TryParse(message.Text, out sdkVersion)).ExitCode == 0)
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

var result = build.Run(new Clean());
if (result.ExitCode != 0) Exit(1);

result = build.Run(
    new MSBuild()
        .WithShortName("Rebuilding the solution")
        .WithProject("MySampleLib.sln")
        .WithTarget("Rebuild")
        .WithRestore(true)
        .WithVerbosity(Verbosity.Normal)
        .AddProps(commonProps));

if (result.ExitCode != 0) Exit(1);

Info($"Running flaky tests with {testAttempts} attempts.");
var failedTests =
    Enumerable.Repeat(new Test().WithNoBuild(true).AddProps(commonProps), testAttempts)
    // Passing an output handler to avoid reporting to CI
    .Select((test, index) => build.Run(test.WithShortName($"Testing (attempt {index + 1})"), _ => {}))
    .TakeWhile(result => result.Summary.FailedTests > 0)
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

result = build.Run(
    new Build()
    .WithShortName($"Building of the {configuration} version")
    .WithConfiguration(configuration)
    .WithOutput(outputDir)
    .WithVerbosity(Verbosity.Normal)
    .AddProps(commonProps));

if (result.ExitCode != 0) Exit(1);

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

if (testInContainerTask.Result.ExitCode != 0) Exit(1);
if (vsTestTask.Result.ExitCode != 0) Exit(1);

result = build.Run(
    new Pack()
        .WithShortName($"The packing of the {configuration} version")
        .WithConfiguration(configuration)
        .WithOutput(outputDir)
        .WithIncludeSymbols(true)
        .WithIncludeSource(true)
        .WithVerbosity(Verbosity.Normal)
        .AddProps(commonProps));

if (result.ExitCode != 0) Exit(1);

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
