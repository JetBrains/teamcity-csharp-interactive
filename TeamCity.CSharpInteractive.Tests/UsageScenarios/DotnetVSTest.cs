// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System.IO;
    using System.Linq;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetVSTest: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=00
            // $description=Test an assembly
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
            var result = build.Run(new Custom("new", "mstest", "-n", "MyTests", "--force"));
            result.Success.ShouldBeTrue();

            // Builds the test project, running a command like: "dotnet build -c Release" from the directory "MyTests"
            result = build.Run(new Build().WithWorkingDirectory("MyTests").WithConfiguration("Release").WithOutput("MyOutput"));
            result.Success.ShouldBeTrue();
            
            // Runs tests via a command like: "dotnet vstest" from the directory "MyTests"
            result = build.Run(
                new VSTest()
                    .AddTestFileNames(Path.Combine("MyOutput", "MyTests.dll"))
                    .WithWorkingDirectory("MyTests"));
            
            // The "result" variable provides details about a build
            result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
            result.Success.ShouldBeTrue();
            // }
        }
    }
}