// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using System.Linq;
    using Docker;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DockerDotnetBuild: Scenario
    {
        [Fact(Skip = "Linux Docker only")]
        //[Fact]
        public void Run()
        {
            // $visible=true
            // $tag=12 Docker API
            // $priority=01
            // $description=Build a project in a docker container
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;
            // Adds the namespace "Docker" to use Docker API
            // ## using Docker;

            // Resolves a build service
            var build = GetService<IBuild>();

            // Creates a base docker command line
            var baseDockerCmd = new Docker.Run()
                .WithImage("mcr.microsoft.com/dotnet/sdk")
                .WithPlatform("linux")
                .WithContainerWorkingDirectory("/MyProjects")
                .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));
            
            // Creates a new library project in a docker container
            var customCmd = new Custom("new", "classlib", "-n", "MyLib", "--force").WithExecutablePath("dotnet");
            var result = build.Run(baseDockerCmd.WithProcess(customCmd));
            result.Success.ShouldBeTrue();

            // Builds the library project in a docker container
            var buildCmd = new Build().WithProject("MyLib/MyLib.csproj").WithExecutablePath("dotnet");
            result = build.Run(baseDockerCmd.WithProcess(buildCmd), output => {});
            
            // The "result" variable provides details about a build
            result.Messages.Any(message => message.State == BuildMessageState.Error).ShouldBeFalse();
            result.Success.ShouldBeTrue();
            // }
        }
    }
}