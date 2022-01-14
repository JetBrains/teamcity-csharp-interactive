// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetClean: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=00
            // $description=Clean a project
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
            var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
            result.Success.ShouldBeTrue();

            // Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
            result = build.Run(new Build().WithWorkingDirectory("MyLib"));
            result.Success.ShouldBeTrue();
            
            // Clean the project, running a command like: "dotnet clean" from the directory "MyLib"
            result = build.Run(new Clean().WithWorkingDirectory("MyLib"));
            
            // The "result" variable provides details about a build
            result.Success.ShouldBeTrue();
            // }
        }
    }
}