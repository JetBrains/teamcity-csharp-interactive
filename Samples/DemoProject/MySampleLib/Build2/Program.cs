// Run this from the working directory where the solution or project to build is located.
using HostApi;
using NuGet.Versioning;

var configuration = Property.Get("configuration", "Release");
var version = NuGetVersion.Parse(Property.Get("version", "1.0.0-dev", true));

var result = new DotNetBuild()
    .WithConfiguration(configuration)
    .AddProps(("version", version.ToString()))
    .Build();

Assertion.Succeed(result);

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
    test.BuildAsync(),
    testInContainer.BuildAsync()
);

Assertion.Succeed(results);