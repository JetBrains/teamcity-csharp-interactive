// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;
using System.Diagnostics.CodeAnalysis;
using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
[Trait("Docker", "true")]
public class DockerDotNetBuildScenario : BaseScenario
{
    [Fact(Skip = "Linux Docker only")]
    [SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped")]
    //[Fact]
    public void Run()
    {
        // $visible=true
        // $tag=12 Docker API
        // $priority=01
        // $description=Build a project in a docker container
        // {
        // Adds the namespace "HostApi" to use .NET build API and Docker API
        // ## using HostApi;

        // Resolves a build service
        var buildRunner = GetService<IBuildRunner>();

        // Creates a base docker command line
        var baseDockerCmd = new DockerRun()
            .WithImage("mcr.microsoft.com/dotnet/sdk")
            .WithPlatform("linux")
            .WithContainerWorkingDirectory("/MyProjects")
            .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));

        // Creates a new library project in a docker container
        var customCmd = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").WithExecutablePath("dotnet");
        var result = buildRunner.Run(baseDockerCmd.WithCommandLine(customCmd));
        result.ExitCode.ShouldBe(0);

        // Builds the library project in a docker container
        var buildCmd = new DotNetBuild().WithProject("MyLib/MyLib.csproj").WithExecutablePath("dotnet");
        result = buildRunner.Run(baseDockerCmd.WithCommandLine(buildCmd), _ => { });

        // The "result" variable provides details about a build
        result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
        result.ExitCode.ShouldBe(0);
        // }
    }
}