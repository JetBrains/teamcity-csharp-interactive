// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetTestScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=01
        // $description=Test a project
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Resolves a build service
        var build = GetService<IBuildRunner>();

        // Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
        var result = build.Run(new DotNetCustom("new", "mstest", "-n", "MyTests", "--force"));
        result.ExitCode.ShouldBe(0);

        // Runs tests via a command like: "dotnet test" from the directory "MyTests"
        result = build.Run(new DotNetTest().WithWorkingDirectory("MyTests"));

        // The "result" variable provides details about a build
        result.ExitCode.ShouldBe(0);
        result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
        // }
    }
}