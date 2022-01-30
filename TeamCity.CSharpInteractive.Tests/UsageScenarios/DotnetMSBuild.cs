// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class DotNetMSBuild : ScenarioHostService
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=11 .NET build API
        // $priority=00
        // $description=Build a project using MSBuild
        // {
        // Adds the namespace "Script.DotNet" to use .NET build API
        // ## using DotNet;

        // Resolves a build service
        var buildRunner = GetService<IBuildRunner>();

        // Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
        var result = buildRunner.Run(new DotNetCustom("new", "classlib", "-n", "MyLib", "--force"));
        result.ExitCode.ShouldBe(0);

        // Builds the library project, running a command like: "dotnet msbuild /t:Build -restore /p:configuration=Release -verbosity=detailed" from the directory "MyLib"
        result = buildRunner.Run(
            new MSBuild()
                .WithWorkingDirectory("MyLib")
                .WithTarget("Build")
                .WithRestore(true)
                .AddProps(("configuration", "Release"))
                .WithVerbosity(DotNetVerbosity.Detailed));

        // The "result" variable provides details about a build
        result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
        result.ExitCode.ShouldBe(0);
        // }
    }
}