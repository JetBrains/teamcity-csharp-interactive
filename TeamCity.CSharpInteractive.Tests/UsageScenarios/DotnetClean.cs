// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetClean: ScenarioHostService
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=00
        // $description=Clean a project
        // {
        // Adds the namespace "Script.DotNet" to use .NET build API
        // ## using DotNet;

        // Resolves a build service
        var buildRunner = GetService<IBuildRunner>();
            
        // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
        var result = buildRunner.Run(new DotNetCustom("new", "classlib", "-n", "MyLib", "--force"));
        result.ExitCode.ShouldBe(0);

        // Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
        result = buildRunner.Run(new HostApi.DotNetBuild().WithWorkingDirectory("MyLib"));
        result.ExitCode.ShouldBe(0);
            
        // Clean the project, running a command like: "dotnet clean" from the directory "MyLib"
        result = buildRunner.Run(new HostApi.DotNetClean().WithWorkingDirectory("MyLib"));
            
        // The "result" variable provides details about a build
        result.ExitCode.ShouldBe(0);
        // }
    }
}