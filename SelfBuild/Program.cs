using Script.DotNet;

const string solutionFile = "TeamCity.CSharpInteractive.sln"; 
if (!File.Exists(solutionFile))
{
    Error($"Cannot find the solution \"{solutionFile}\". Current directory is \"{Environment.CurrentDirectory}\".");
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

var buildProps = new[]
{
    ("version", nextToolVersion.ToString())
};

var build = GetService<IBuild>();

var buildSolution = new Build()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);

var result = build.Run(buildSolution);
if (result.ExitCode != 0) return 1;

var test = new Test()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithNoBuild(true)
    .WithProps(buildProps);

result = build.Run(test);
if (result.ExitCode != 0 || result.Summary.FailedTests != 0) return 1;

var pack = new Pack()
    .WithProject(solutionFile)
    .WithConfiguration(configuration)
    .WithProps(buildProps);
    
result = build.Run(pack);
return result.ExitCode ?? 1;