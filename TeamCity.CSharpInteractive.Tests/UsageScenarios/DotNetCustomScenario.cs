// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetCustomScenario : ScenarioHostService
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=00
        // $description=Run a custom .NET command
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Resolves a build service
        var buildRunner = GetService<IBuildRunner>();

        // Gets the dotnet version, running a command like: "dotnet --version"
        Version? version = default;
        var result = buildRunner.Run(
            new DotNetCustom("--version"),
            message => Version.TryParse(message.Text, out version));

        result.ExitCode.ShouldBe(0);
        version.ShouldNotBeNull();
        // }
    }
}