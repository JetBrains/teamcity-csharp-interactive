using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NuGet.Versioning;
using Script.Docker;
using Script.DotNet;
using Script.NuGet;

var currentDirectory = Environment.CurrentDirectory;
var outputDir = Path.Combine(currentDirectory, "bin");
Host.WriteLine($"Starting {Args[0]} at \"{currentDirectory}\".");

if (!Props.TryGetValue("configuration", out var configuration)) configuration = "Release";

const string packageId = "MySampleLib";
if (!Props.TryGetValue("version", out var packageVersion))
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

var props = new[] { ("Version", packageVersion) };

var buildRunner = GetService<IBuildRunner>();

var requiredSdkVersion = new Version(6, 0);
Info($"Checking the .NET SDK version {requiredSdkVersion}.");
Version? sdkVersion = default;
if (
    buildRunner.Run(new Custom("--version"), message => Version.TryParse(message.Text, out sdkVersion)).ExitCode == 0
    && sdkVersion != default
    && sdkVersion < requiredSdkVersion)
{
    Error($".NET SDK {requiredSdkVersion} or newer is required.");
    return 1;
}

if (buildRunner.Run(new Clean()).ExitCode != 0)
    return 1;

if (buildRunner.Run(new MSBuild()
        .WithShortName("Rebuilding the solution")
        .WithProject("MySampleLib.sln")
        .WithTarget("Rebuild")
        .WithRestore(true)
        .WithVerbosity(Verbosity.Normal)
        .AddProps(props)).ExitCode != 0)
    return 1;

if (buildRunner.Run(new Build()
        .WithShortName($"Building of the {configuration} version")
        .WithConfiguration(configuration)
        .WithOutput(outputDir)
        .WithVerbosity(Verbosity.Normal)
        .AddProps(props)).ExitCode != 0)
    return 1;

var vstest = new VSTest()
    .WithTestFileNames(Path.Combine(outputDir, "MySampleLib.Tests.dll"));

var test = new Test()
    .WithExecutablePath("dotnet")
    .WithNoBuild(true)
    .WithVerbosity(Verbosity.Normal);

var testInContainer = new Script.Docker.Run(test, $"mcr.microsoft.com/dotnet/sdk:{requiredSdkVersion}")
    .WithPlatform("linux")
    .AddVolumes((currentDirectory, "/project"))
    .WithContainerWorkingDirectory("/project");

var results = await Task.WhenAll(
    buildRunner.RunAsync(testInContainer),
    buildRunner.RunAsync(test),
    buildRunner.RunAsync(vstest));

if (results.Any(i => i.ExitCode != 0)) return 1;

if (buildRunner.Run(
        new Pack()
            .WithShortName($"The packing of the {configuration} version")
            .WithConfiguration(configuration)
            .WithOutput(outputDir)
            .WithIncludeSymbols(true)
            .WithIncludeSource(true)
            .WithVerbosity(Verbosity.Normal)
            .AddProps(props)).ExitCode != 0)
    return 1;

Info("Publishing artifacts.");
var teamCityWriter = GetService<ITeamCityWriter>();

(
    from packageExtension in new[] {"nupkg", "symbols.nupkg"}
    let path = Path.Combine(outputDir, $"{packageId}.{packageVersion}.{packageExtension}")
    select $"{path} => .")
    .ToList()
    .ForEach(artifact => teamCityWriter.PublishArtifact(artifact));

return 0;