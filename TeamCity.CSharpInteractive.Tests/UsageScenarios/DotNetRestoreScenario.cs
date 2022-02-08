// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetRestoreScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=01
        // $description=Restore a project
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
        var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
        result.ExitCode.ShouldBe(0);

        // Restore the project, running a command like: "dotnet restore" from the directory "MyLib"
        result = new DotNetRestore().WithWorkingDirectory("MyLib").Build();
        result.ExitCode.ShouldBe(0);
        // }
    }
}