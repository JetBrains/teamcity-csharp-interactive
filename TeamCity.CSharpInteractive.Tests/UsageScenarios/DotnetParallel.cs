// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System.Linq;
    using System.Threading.Tasks;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetParallel: Scenario
    {
        [Fact]
        public async Task Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=01
            // $description=Parallel builds
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Creates a new test project, running a command like: "dotnet new mstest -n MSTestTests --force"
            var createMSTestTask = build.RunAsync(new Custom("new", "mstest", "-n", "MSTestTests", "--force"));
            
            // Runs tests via a command like: "dotnet test" from the directory "MSTestTests"
            var runMSTestTask = build.RunAsync(new Test().WithWorkingDirectory("MSTestTests"));
            
            // Creates a another test project, running a command like: "dotnet new xunit -n XUnitTests --force"
            var createXUnitTask = build.RunAsync(new Custom("new", "xunit", "-n", "XUnitTests", "--force"));
            
            // Runs tests via a command like: "dotnet test" from the directory "XUnitTests"
            var runXUnitTask = build.RunAsync(new Test().WithWorkingDirectory("XUnitTests"));
            
            // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
            var createLibTask = build.RunAsync(new Custom("new", "classlib", "-n", "MyLib", "--force"));

            // Publish the project, running a command like: "dotnet publish --framework net6.0" from the directory "MyLib"
            var publishLibTask = build.RunAsync(new Publish().WithWorkingDirectory("MyLib").WithFramework("net6.0"));

            // Runs pipelines in parallel
            var results = await Task.WhenAll(
                // MSTest tests pipeline
                createMSTestTask.ContinueWith(_ => runMSTestTask.Result),
                // XUnit tests pipeline
                createXUnitTask.ContinueWith(_ => runXUnitTask.Result),
                // Publish pipeline
                createLibTask.ContinueWith(_ => publishLibTask.Result));
            
            // The "results" variable provides details about all builds
            results.Length.ShouldBe(3);
            results.All(result => result.Success).ShouldBeTrue();
            // }
        }
    }
}