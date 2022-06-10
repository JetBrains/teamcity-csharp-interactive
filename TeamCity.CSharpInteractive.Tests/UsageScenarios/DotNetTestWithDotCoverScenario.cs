// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetTestWithDotCoverScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=01
        // $description=Run tests under dotCover
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
        var exitCode = new DotNetNew("mstest", "-n", "MyTests", "--force").Run();
        exitCode.ShouldBe(0);

        exitCode = new DotNetNew("tool-manifest")
            .WithWorkingDirectory("MyTests")
            .Run();
        exitCode.ShouldBe(0);
        
        exitCode = new DotNetCustom("tool",  "install", "--local", "JetBrains.dotCover.GlobalTool")
            .WithWorkingDirectory("MyTests")
            .Run();
        exitCode.ShouldBe(0);
        
        var result =
            // Creates a test command
            new DotNetTest().WithWorkingDirectory("MyTests")
            // Modifies the test command by putting "dotCover" in front of all arguments to have something like "dotnet dotcover test ..."
            .Customize(cmd => cmd.WithArgs("dotcover").AddArgs(cmd.Args))
            // Runs tests via a command like: "dotnet test" from the directory "MyTests"
            .Build();

        // The "result" variable provides details about a build
        result.ExitCode.ShouldBe(0);
        result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
        
        // Generates a JSON code coverage report.
        exitCode = new DotNetCustom("dotCover", "report", "--source=dotCover.Output.dcvr", "--reportType=HTML")
            .WithWorkingDirectory("MyTests")
            .Run();
        exitCode.ShouldBe(0);
        
        // Check for a dotCover report
        File.Exists(Path.Combine("MyTests", "dotCover.Output.html")).ShouldBeTrue();
        // }
    }
}