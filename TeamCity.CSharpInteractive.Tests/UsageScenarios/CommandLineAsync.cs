// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using Cmd;
    
[CollectionDefinition("Integration", DisableParallelization = true)]
public class CommandLineAsync: Scenario
{
    [SkippableFact]
    public async Task Run()
    {
        Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=02
        // $description=Run a command line asynchronously
        // {
        // Adds the namespace "Cmd" to use Command Line API
        // ## using Cmd;

        int? exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
        // }
            
        exitCode.HasValue.ShouldBeTrue();
    }
}