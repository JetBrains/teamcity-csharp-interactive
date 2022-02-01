using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NuGet.Versioning;

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

var props = new[] {("Version", packageVersion)};

var buildRunner = GetService<IBuildRunner>();

var requiredSdkVersion = new Version(6, 0);
Info($"Checking the .NET SDK version {requiredSdkVersion}.");
Version? sdkVersion = default;
if (
    buildRunner.Run(new DotNetCustom("--version"), message => Version.TryParse(message.Text, out sdkVersion)).ExitCode == 0
    && sdkVersion != default
    && sdkVersion < requiredSdkVersion)
{
    Error($".NET SDK {requiredSdkVersion} or newer is required.");
    return 1;
}

if (buildRunner.Run(new MSBuild()
        .WithShortName("Rebuilding solution")
        .AddProps(("configuration", configuration))
        .WithProject("MySampleLib.sln")
        .WithTarget("Rebuild")
        .WithRestore(true)
        .WithVerbosity(DotNetVerbosity.Normal)
        .AddProps(props)).ExitCode != 0)
    return 1;

if (buildRunner.Run(new DotNetBuild()
        .WithShortName($"Building {configuration} version")
        .WithConfiguration(configuration)
        .WithOutput(outputDir)
        .WithVerbosity(DotNetVerbosity.Normal)
        .AddProps(props)).ExitCode != 0)
    return 1;

var vstest = new VSTest()
    .WithTestFileNames(Path.Combine(outputDir, "MySampleLib.Tests.dll"));

var test = new DotNetTest()
    .WithExecutablePath("dotnet")
    .WithNoBuild(true)
    .WithVerbosity(DotNetVerbosity.Normal);

var testInContainer = new DockerRun(test, $"mcr.microsoft.com/dotnet/sdk:{requiredSdkVersion}")
    .WithPlatform("linux")
    .AddVolumes((currentDirectory, "/project"))
    .WithContainerWorkingDirectory("/project");

var results = await Task.WhenAll(
    buildRunner.RunAsync(testInContainer),
    buildRunner.RunAsync(test),
    buildRunner.RunAsync(vstest));

if (results.Any(i => i.ExitCode != 0)) return 1;

if (buildRunner.Run(
        new DotNetPack()
            .WithShortName($"Packing {configuration} version")
            .WithConfiguration(configuration)
            .WithOutput(outputDir)
            .WithIncludeSymbols(true)
            .WithIncludeSource(true)
            .WithVerbosity(DotNetVerbosity.Normal)
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