// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetCleanScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=01
        // $description=Clean a project
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
        var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
        result.ExitCode.ShouldBe(0);

        // Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
        result = new DotNetBuild().WithWorkingDirectory("MyLib").Build();
        result.ExitCode.ShouldBe(0);

        // Clean the project, running a command like: "dotnet clean" from the directory "MyLib"
        result = new DotNetClean().WithWorkingDirectory("MyLib").Build();

        // The "result" variable provides details about a build
        result.ExitCode.ShouldBe(0);
        // }
    }
}