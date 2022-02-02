// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
[Trait("Docker", "true")]
public class DockerRun : ScenarioHostService
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=12 Docker API
        // $priority=00
        // $description=Running in docker
        // {
        // Adds the namespace "HostApi" to use Command Line API and Docker API
        // ## using HostApi;
        
        // Resolves a build service
        var commandLineRunner = GetService<ICommandLineRunner>();

        // Creates some command line to run in a docker container
        var cmd = new CommandLine("whoami");

        // Runs the command line in a docker container
        var result = commandLineRunner.Run(new HostApi.DockerRun(cmd, "mcr.microsoft.com/dotnet/sdk").WithAutoRemove(true));
        result.ShouldBe(0);
        // }
    }
}