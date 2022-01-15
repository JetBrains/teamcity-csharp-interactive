using Cmd;
using Docker;
using Dotnet;
using TeamCity.CSharpInteractive.Contracts;

if (!Props.TryGetValue("version", out var version)) version = "1.0.0-dev";
if (!Props.TryGetValue("configuration", out var configuration)) configuration = "Release";

var build = new Build()
    .WithShortName("Building")
    .WithConfiguration(configuration)
    .AddProps(("version", version));

var tests = new Test()
    .WithShortName("Testing")
    .WithConfiguration(configuration)
    .WithNoBuild(true);

var docker = new Docker.Run()
    .WithImage("mcr.microsoft.com/dotnet/sdk")
    .WithPlatform("linux")
    .WithContainerWorkingDirectory("/MyProjects")
    .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));

var testsInDocker = docker.WithProcess(tests.WithExecutablePath("dotnet"));

var pack = new Build()
    .WithShortName("Creating NuGet package")
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
        }
    }
}

var buildService = GetService<IBuild>();

// Synchronous
Check(buildService.Run(build));
Check(buildService.Run(tests));
Check(buildService.Run(testsInDocker));
Check(buildService.Run(pack));

// Asynchronous
Check(await buildService.RunAsync(build));
Check(await Task.WhenAll(buildService.RunAsync(tests), buildService.RunAsync(testsInDocker)));
Check(await buildService.RunAsync(pack));

// Pipeline
Check(
    await buildService.RunAsync(build)
        .ContinueWith(Task.WhenAll(
            buildService.RunAsync(tests),
            buildService.RunAsync(testsInDocker)))
        .ContinueWith(buildService.RunAsync(pack))
    );