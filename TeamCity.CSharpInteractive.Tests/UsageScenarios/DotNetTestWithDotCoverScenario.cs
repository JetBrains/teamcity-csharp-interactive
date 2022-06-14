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

        // Creates the tool manifest and installs the dotCover tool locally
        // It is better to run the following 2 commands manually
        // and commit these changes to a source control
        exitCode = new DotNetNew("tool-manifest").Run();
        exitCode.ShouldBe(0);
        
        exitCode = new DotNetCustom("tool",  "install", "--local", "JetBrains.dotCover.GlobalTool").Run();
        exitCode.ShouldBe(0);
        
        var dotCoverSnapshot = Path.Combine("MyTests", "dotCover.dcvr");
        var dotCoverReport = Path.Combine("MyTests", "dotCover.html");
        var result =
            // Creates a test command
            new DotNetTest().WithProject("MyTests")
            // Modifies the test command by putting "dotCover" in front of all arguments
            // to have something like "dotnet dotcover test ..."
            // and adding few specific arguments to the end
            .Customize(cmd => 
                cmd.WithArgs("dotcover")
                + cmd.Args
                + $"--dcOutput={dotCoverSnapshot}"
                + "--dcFilters=+:module=TeamCity.CSharpInteractive.HostApi;+:module=dotnet-csi"
                + "--dcAttributeFilters=System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")
            // Runs tests via a command like: "dotnet test" from the directory "MyTests"
            .Build();

        // The "result" variable provides details about a build
        result.ExitCode.ShouldBe(0);
        result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
        
        // Generates a HTML code coverage report.
        exitCode = new DotNetCustom("dotCover", "report", $"--source={dotCoverSnapshot}", $"--output={dotCoverReport}", "--reportType=HTML").Run();
        exitCode.ShouldBe(0);
        
        // Check for a dotCover report
        File.Exists(dotCoverReport).ShouldBeTrue();
        // }
    }
}