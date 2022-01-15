// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using Cmd;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetPublish: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=00
            // $description=Publish a project
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
            var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
            result.State.ShouldBe(BuildState.Succeeded);

            // Publish the project, running a command like: "dotnet publish --framework net6.0" from the directory "MyLib"
            result = build.Run(new Publish().WithWorkingDirectory("MyLib").WithFramework("net6.0"));
            result.State.ShouldBe(BuildState.Succeeded);
            // }
        }
    }
}