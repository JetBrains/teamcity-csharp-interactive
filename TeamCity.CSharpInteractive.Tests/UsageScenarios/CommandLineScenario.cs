// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;
using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class CommandLineScenario : BaseScenario
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

        var exitCode = GetService<ICommandLineRunner>().Run(new CommandLine("cmd", "/c", "DIR"));
        exitCode.ShouldBe(0);
        
        // or the same thing using the extension method
        exitCode = new CommandLine("cmd", "/c", "DIR").Run();
        exitCode.ShouldBe(0);
        
        // using operator '+'
        var cmd = new CommandLine("cmd") + "/c" + "DIR";
        exitCode = cmd.Run();
        exitCode.ShouldBe(0);
        
        // with environment variables
        cmd = new CommandLine("cmd") + "/c" + "DIR" + ("MyEnvVar", "Some Value");
        exitCode = cmd.Run();
        exitCode.ShouldBe(0);
        // }
    }
}