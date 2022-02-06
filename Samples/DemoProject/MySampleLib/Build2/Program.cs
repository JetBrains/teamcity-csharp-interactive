using HostApi;
using NuGet.Versioning;

var configuration = GetProperty("configuration", "Release");
var version = NuGetVersion.Parse(GetProperty("version", "1.0.0-dev", true));

var runner = GetService<IBuildRunner>();

var build = new DotNetBuild()
    .WithConfiguration(configuration)
    .AddProps(("version", version.ToString()));

if (runner.Run(build).ExitCode != 0)
{
    Error("Build failed.");
    return 1;
}

var test = new DotNetTest()
    .WithConfiguration(configuration)
    .WithNoBuild(true);

var testInContainer = new DockerRun(
        test.WithExecutablePath("dotnet"),
        $"mcr.microsoft.com/dotnet/sdk:6.0")
    .WithPlatform("linux")
    .AddVolumes((Environment.CurrentDirectory, "/project"))
    .WithContainerWorkingDirectory("/project");

var results = await Task.WhenAll(
    runner.RunAsync(test),
    runner.RunAsync(testInContainer)
);

if (results.Any(result => result.ExitCode != 0))
{
    var failedTests =
        from buildResult in results
        from testResult in buildResult.Tests
        where testResult.State == TestState.Failed
        select testResult.ToString();

    foreach (var failedTest in failedTests.Distinct())
    {
        Error(failedTest);
    }

    Error("Tests failed.");
    return 1;
}

return 0;

string GetProperty(string name, string defaultProp, bool showWarning = false)
{
    if (Props.TryGetValue(name, out var prop) && !string.IsNullOrWhiteSpace(prop))
    {
        return prop;
    }

    var message = $"The property \"{name}\" was not defined, the default value \"{defaultProp}\" was used.";
    if (showWarning)
    {
        Warning(message);
    }
    else
    {
        Info(message);
    }

    return defaultProp;
}