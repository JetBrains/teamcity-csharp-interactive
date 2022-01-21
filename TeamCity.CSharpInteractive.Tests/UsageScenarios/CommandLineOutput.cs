// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using Script.Cmd;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class CommandLineOutput: ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);
        Skip.IfNot(string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=10 Command Line API
        // $priority=04
        // $description=Run and process output
        // {
        // Adds the namespace "Script.Cmd" to use Command Line API
        // ## using Cmd;

        var lines = new System.Collections.Generic.List<string>();
        int? exitCode = GetService<ICommandLineRunner>().Run(
            new CommandLine("cmd").AddArgs("/c", "SET").AddVars(("MyEnv", "MyVal")),
            i => lines.Add(i.Line));
            
        lines.ShouldContain("MyEnv=MyVal");
        // }
            
        exitCode.HasValue.ShouldBeTrue();
    }
}