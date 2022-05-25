// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;
using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class CommandLineOutputScenario : BaseScenario
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=04
        // $description=Run and process output
        // {
        // Adds the namespace "HostApi" to use Command Line API
        // ## using HostApi;

        var lines = new List<string>();
        int? exitCode = new CommandLine("cmd", "/c", "SET")
            .AddVars(("MyEnv", "MyVal"))
            .Run(output => lines.Add(output.Line));

        lines.ShouldContain("MyEnv=MyVal");
        // }

        exitCode.HasValue.ShouldBeTrue();
    }
}