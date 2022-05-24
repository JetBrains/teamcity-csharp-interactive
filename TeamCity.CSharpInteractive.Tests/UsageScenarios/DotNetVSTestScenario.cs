// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetVSTestScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=01
        // $description=Test an assembly
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
        var result = new DotNetNew("mstest", "-n", "MyTests", "--force").Build();
        result.ExitCode.ShouldBe(0);

        // Builds the test project, running a command like: "dotnet build -c Release" from the directory "MyTests"
        result = new DotNetBuild().WithWorkingDirectory("MyTests").WithConfiguration("Release").WithOutput("MyOutput").Build();
        result.ExitCode.ShouldBe(0);

        // Runs tests via a command like: "dotnet vstest" from the directory "MyTests"
        result = new VSTest()
            .AddTestFileNames(Path.Combine("MyOutput", "MyTests.dll"))
            .WithWorkingDirectory("MyTests")
            .Build();

        // The "result" variable provides details about a build
        result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
        result.ExitCode.ShouldBe(0);
        // }
    }
}