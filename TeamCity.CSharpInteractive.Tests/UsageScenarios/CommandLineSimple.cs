// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using Cmd;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class CommandLineSimple: Scenario
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
        // Adds the namespace "Cmd" to use Command Line API
        // ## using Cmd;

        int? exitCode = GetService<ICommandLine>().Run(new CommandLine("whoami", "/all"));
        // }
            
        exitCode.HasValue.ShouldBeTrue();
    }
}