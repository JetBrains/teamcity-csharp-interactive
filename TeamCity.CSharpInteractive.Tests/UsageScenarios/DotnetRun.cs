// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using CSharpInteractive;
using DotNet;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class DotNetRun: Scenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=00
        // $description=Run a project
        // {
        // Adds the namespace "DotNet" to use .NET build API
        // ## using DotNet;

        // Resolves a build service
        var build = GetService<IBuild>();
            
        // Creates a new console project, running a command like: "dotnet new console -n MyApp --force"
        var result = build.Run(new Custom("new", "console", "-n", "MyApp", "--force"));
        result.ExitCode.ShouldBe(0);

        // Runs the console project using a command like: "dotnet run" from the directory "MyApp"
        var stdOut = new List<string>(); 
        result = build.Run(new Run().WithWorkingDirectory("MyApp"), message => stdOut.Add(message.Text));
        result.ExitCode.ShouldBe(0);
            
        // Checks StdOut
        stdOut.ShouldBe(new []{ "Hello, World!" });
        // }
    }
}