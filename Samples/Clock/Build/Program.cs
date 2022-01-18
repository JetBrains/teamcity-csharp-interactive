using Cmd;
using Docker;
using Dotnet;
using TeamCity.CSharpInteractive.Contracts;

if (!Props.TryGetValue("version", out var version)) version = "1.0.0-dev";
if (!Props.TryGetValue("configuration", out var configuration)) configuration = "Release";

var compile = new Build()
    .WithShortName("Compiling")
    .WithConfiguration(configuration)
    .AddProps(("version", version));

var test = new Test()
    .WithShortName("Testing")
    .WithConfiguration(configuration)
    .WithNoBuild(true);

var docker = new Docker.Run()
    .WithShortName("Testing in container")
    .WithImage("mcr.microsoft.com/dotnet/sdk")
    .WithPlatform("linux")
    .WithContainerWorkingDirectory("/MyProjects")
    .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));

var testInContainer = docker.WithProcess(test.WithExecutablePath("dotnet"));

var pack = new Build()
    .WithShortName("Creating packages")
    .WithConfiguration(configuration)
    .AddProps(("version", version));

void Check(params BuildResult[] results)
{
    foreach (var result in results)
    {
        if (result.State == BuildState.Succeeded)
        {
            WriteLine(result, Color.Success);
        }
        else
        {
            Error(result);
            Environment.Exit(1);
        }
    }
}

var build = GetService<IBuild>();

// Synchronous
Check(build.Run(compile));
Check(build.Run(test));
Check(build.Run(testInContainer));
Check(build.Run(pack));

// Asynchronous
Check(await build.RunAsync(compile));
Check(await Task.WhenAll(build.RunAsync(test), build.RunAsync(testInContainer)));
Check(await build.RunAsync(pack));