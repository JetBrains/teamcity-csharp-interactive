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
    //[Fact(Skip = "Linux Docker only")]
    [SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped")]
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=12 Docker API
        // $priority=01
        // $description=Build a project in a docker container
        // {
        // Adds the namespace "HostApi" to use .NET build API and Docker API
        // ## using HostApi;
        
        // Creates a base docker command line
        var dockerRun = new DockerRun()
            .WithAutoRemove(true)
            .WithImage("mcr.microsoft.com/dotnet/sdk")
            .WithPlatform("linux")
            .WithContainerWorkingDirectory("/MyProjects")
            .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));

        // Creates a new library project in a docker container
        var exitCode = dockerRun
            .WithCommandLine(new DotNetCustom("new", "classlib", "-n", "MyLib", "--force"))
            .Run();

        exitCode.ShouldBe(0);

        // Builds the library project in a docker container
        var result = dockerRun
            .WithCommandLine(new DotNetBuild().WithProject("MyLib/MyLib.csproj"))
            .Build();

        // The "result" variable provides details about a build
        result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
        result.ExitCode.ShouldBe(0);
        // }
    }
}