// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System.Linq;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetTest: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=00
            // $description=Test a project
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
            var result = build.Run(new Custom("new", "mstest", "-n", "MyTests", "--force"));
            result.Success.ShouldBeTrue();

            // Runs tests via a command like: "dotnet test" from the directory "MyTests"
            result = build.Run(new Test().WithWorkingDirectory("MyTests"));
            
            // The "result" variable provides details about a build
            result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
            result.Success.ShouldBeTrue();
            // }
        }
    }
}