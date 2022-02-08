// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetPublishScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=01
        // $description=Publish a project
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;

        // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
        var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
        result.ExitCode.ShouldBe(0);

        // Publish the project, running a command like: "dotnet publish --framework net6.0" from the directory "MyLib"
        result = new DotNetPublish().WithWorkingDirectory("MyLib").WithFramework("net6.0").Build();
        result.ExitCode.ShouldBe(0);
        // }
    }
}