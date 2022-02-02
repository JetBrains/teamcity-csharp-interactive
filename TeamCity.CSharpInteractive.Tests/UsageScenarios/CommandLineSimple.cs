// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;
using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class CommandLineSimple : ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=01
        // $description=Run a command line
        // {
        // Adds the namespace "HostApi" to use Command Line API
        // ## using HostApi;

        int? exitCode = GetService<ICommandLineRunner>().Run(new CommandLine("cmd", "/c", "DIR"));
        // }

        exitCode.HasValue.ShouldBeTrue();
    }
}