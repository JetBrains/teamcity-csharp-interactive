// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
[Trait("Docker", "true")]
public class DockerRunScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=12 Docker API
        // $priority=01
        // $description=Running in docker
        // {
        // Adds the namespace "HostApi" to use Command Line API and Docker API
        // ## using HostApi;
        
        // Creates some command line to run in a docker container
        var cmd = new CommandLine("whoami");

        // Runs the command line in a docker container
        var result = new DockerRun(cmd, "mcr.microsoft.com/dotnet/sdk")
            .WithAutoRemove(true)
            .Run();

        result.ShouldBe(0);
        // }
    }
}