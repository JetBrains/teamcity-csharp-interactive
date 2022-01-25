// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class CommandLineWithTimeout: ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=06
        // $description=Run timeout
        // $header=If timeout expired a process will be killed.
        // {
        // Adds the namespace "Script.Cmd" to use Command Line API
        // ## using Cmd;

        int? exitCode = GetService<ICommandLineRunner>().Run(
            new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
            default,
            TimeSpan.FromMilliseconds(1));
            
        exitCode.HasValue.ShouldBeFalse();
        // }
    }
}