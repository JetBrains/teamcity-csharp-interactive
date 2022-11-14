// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetBuildScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=01
        // $description=Build a project
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
        var result = new DotNetNew("xunit", "-n", "MyLib", "--force").Build();
        result.ExitCode.ShouldBe(0);

        // Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
        result = new DotNetBuild().WithWorkingDirectory("MyLib").Build();

        // The "result" variable provides details about a build
        result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
        result.ExitCode.ShouldBe(0);
        
        // Runs tests in docker
        result = new DotNetTest().WithWorkingDirectory("MyLib").Build();
        result.ExitCode.ShouldBe(0);
        result.Summary.Tests.ShouldBe(1);
        result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
        // }
    }
}