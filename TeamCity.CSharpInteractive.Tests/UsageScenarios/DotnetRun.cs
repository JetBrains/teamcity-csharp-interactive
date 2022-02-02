// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetRun : ScenarioHostService
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=00
        // $description=Run a project
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Resolves a build service
        var buildRunner = GetService<IBuildRunner>();

        // Creates a new console project, running a command like: "dotnet new console -n MyApp --force"
        var result = buildRunner.Run(new DotNetCustom("new", "console", "-n", "MyApp", "--force"));
        result.ExitCode.ShouldBe(0);

        // Runs the console project using a command like: "dotnet run" from the directory "MyApp"
        var stdOut = new List<string>();
        result = buildRunner.Run(new HostApi.DotNetRun().WithWorkingDirectory("MyApp"), message => stdOut.Add(message.Text));
        result.ExitCode.ShouldBe(0);

        // Checks StdOut
        stdOut.ShouldBe(new[] {"Hello, World!"});
        // }
    }
}