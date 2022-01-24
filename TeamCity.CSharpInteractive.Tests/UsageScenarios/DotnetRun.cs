// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using Script.DotNet;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetRun: ScenarioHostService
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=00
        // $description=Run a project
        // {
        // Adds the namespace "Script.DotNet" to use .NET build API
        // ## using DotNet;

        // Resolves a build service
        var buildRunner = GetService<IBuildRunner>();
            
        // Creates a new console project, running a command like: "dotnet new console -n MyApp --force"
        var result = buildRunner.Run(new Custom("new", "console", "-n", "MyApp", "--force"));
        result.ExitCode.ShouldBe(0);

        // Runs the console project using a command like: "dotnet run" from the directory "MyApp"
        var stdOut = new List<string>(); 
        result = buildRunner.Run(new Run().WithWorkingDirectory("MyApp"), message => stdOut.Add(message.Text));
        result.ExitCode.ShouldBe(0);
            
        // Checks StdOut
        stdOut.ShouldBe(new []{ "Hello, World!" });
        // }
    }
}