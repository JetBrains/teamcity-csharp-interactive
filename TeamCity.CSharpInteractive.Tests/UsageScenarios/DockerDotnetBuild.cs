// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System.Diagnostics.CodeAnalysis;
using Script.Docker;
using Script.DotNet;
using Run = Script.Docker.Run;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class DockerDotNetBuild: ScenarioHostService
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
        // Adds the namespace "Script.DotNet" to use .NET build API
        // ## using DotNet;
        // Adds the namespace "Script.Docker" to use Docker API
        // ## using Docker;

        // Resolves a build service
        var build = GetService<IBuildRunner>();

        // Creates a base docker command line
        var baseDockerCmd = new Run()
            .WithImage("mcr.microsoft.com/dotnet/sdk")
            .WithPlatform("linux")
            .WithContainerWorkingDirectory("/MyProjects")
            .AddVolumes((System.Environment.CurrentDirectory, "/MyProjects"));
            
        // Creates a new library project in a docker container
        var customCmd = new Custom("new", "classlib", "-n", "MyLib", "--force").WithExecutablePath("dotnet");
        var result = build.Run(baseDockerCmd.WithCommandLine(customCmd));
        result.ExitCode.ShouldBe(0);

        // Builds the library project in a docker container
        var buildCmd = new Build().WithProject("MyLib/MyLib.csproj").WithExecutablePath("dotnet");
        result = build.Run(baseDockerCmd.WithCommandLine(buildCmd), _ => {});
            
        // The "result" variable provides details about a build
        result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
        result.ExitCode.ShouldBe(0);
        // }
    }
}