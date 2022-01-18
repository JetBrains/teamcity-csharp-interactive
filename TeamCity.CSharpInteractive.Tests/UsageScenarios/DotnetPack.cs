// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using Cmd;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetPack: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=00
            // $description=Pack a project
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
            var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
            result.State.ShouldBe(BuildState.Succeeded);

            // Creates a NuGet package of version 1.2.3 for the project, running a command like: "dotnet pack /p:version=1.2.3" from the directory "MyLib"
            result = build.Run(
                new Pack()
                    .WithWorkingDirectory("MyLib")
                    .AddProps(("version", "1.2.3")));

            result.State.ShouldBe(BuildState.Succeeded);
            // }
        }
    }
}