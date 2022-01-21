// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using Script.Cmd;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class CommandLineSimple: ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=01
        // $description=Run a command line
        // {
        // Adds the namespace "Script.Cmd" to use Command Line API
        // ## using Cmd;

        int? exitCode = GetService<ICommandLineRunner>().Run(new CommandLine("whoami", "/all"));
        // }
            
        exitCode.HasValue.ShouldBeTrue();
    }
}