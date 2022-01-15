// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System.Threading.Tasks;
    using Cmd;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetPipelines: Scenario
    {
        [Fact]
        public async Task Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=01
            // $description=Pipelines
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();

            var runMSTests = 
                build.RunAsync(new Custom("new", "mstest", "-n", "MSTestTests", "--force"))
                .ContinueWith(build.RunAsync(new Test().WithWorkingDirectory("MSTestTests")));
            
            var runXUnitTests = 
                build.RunAsync(new Custom("new", "xunit", "-n", "XUnitTests", "--force"))
                .ContinueWith(build.RunAsync(new Test().WithWorkingDirectory("XUnitTests")));

            var pack = 
                build.RunAsync(new Custom("new", "classlib", "-n", "MyLib", "--force"))
                .ContinueWith(build.RunAsync(new Build().WithWorkingDirectory("MyLib")))
                .ContinueWith(build.RunAsync(new Pack().WithWorkingDirectory("MyLib").WithNoBuild(true)));
            
            // Creates NuGet package after running all tests in parallel, when all tests pass
            var result = await Task.WhenAll(runMSTests, runXUnitTests)
                .ContinueWith(pack, previousBuild => previousBuild.State == BuildState.Succeeded && previousBuild.Totals.FailedTests == 0);

            result.State.ShouldBe(BuildState.Succeeded);
            // }
        }
    }
}