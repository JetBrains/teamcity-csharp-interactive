// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;
using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class CommandLineInParallelScenario : BaseScenario
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=05
        // $description=Run asynchronously in parallel
        // {
        // Adds the namespace "HostApi" to use Command Line API
        // ## using HostApi;

        Task<int?> task = GetService<ICommandLineRunner>().RunAsync(new CommandLine("cmd", "/c", "DIR"));
        int? exitCode = new CommandLine("cmd", "/c", "SET").Run();
        task.Wait();
        // }

        task.Result.HasValue.ShouldBeTrue();
        exitCode.HasValue.ShouldBeTrue();
    }
}