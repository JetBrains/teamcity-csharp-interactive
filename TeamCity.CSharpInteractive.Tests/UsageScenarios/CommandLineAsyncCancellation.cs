// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;
using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class CommandLineAsyncCancellation : ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=06
        // $description=Cancellation of asynchronous run
        // $header=The cancellation will kill a related process.
        // {
        // Adds the namespace "Script.Cmd" to use Command Line API
        // ## using Cmd;

        var cancellationTokenSource = new CancellationTokenSource();
        Task<int?> task = GetService<ICommandLineRunner>().RunAsync(
            new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
            default,
            cancellationTokenSource.Token);

        cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));
        task.IsCompleted.ShouldBeFalse();
        // }
    }
}