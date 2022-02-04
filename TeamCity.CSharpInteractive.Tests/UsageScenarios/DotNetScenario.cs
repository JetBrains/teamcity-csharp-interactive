// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=00
        // $description=.NET Scenarios
        // {
        // Adds the namespace "HostApi" to use .NET build API
        // ## using HostApi;
        
        var clean = new DotNetClean();
        
        var restore = new DotNetRestore()
            .WithWorkingDirectory("MyLib");

        var build = new DotNetBuild()
            .WithConfiguration("Release");

        // Resolves a build service
        var buildRunner = GetService<IBuildRunner>();
        // }
    }
}