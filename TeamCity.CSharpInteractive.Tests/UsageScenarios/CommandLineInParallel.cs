// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using Cmd;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class CommandLineInParallel: ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=05
        // $description=Run asynchronously in parallel
        // {
        // Adds the namespace "Cmd" to use Command Line API
        // ## using Cmd;

        Task<int?> task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
        int? exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
        task.Wait();
        // }
            
        task.Result.HasValue.ShouldBeTrue();
        exitCode.HasValue.ShouldBeTrue();
    }
}