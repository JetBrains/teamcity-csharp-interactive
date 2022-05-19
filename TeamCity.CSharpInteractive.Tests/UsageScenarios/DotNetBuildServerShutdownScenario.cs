// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetBuildServerShutdownScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=02
        // $description=Shuts down build servers
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Shuts down all build servers that are started from dotnet.
        var exitCode = new DotNetBuildServerShutdown().Run();

        exitCode.ShouldBe(0);
        // }
    }
}