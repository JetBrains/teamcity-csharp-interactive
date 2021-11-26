// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Cmd;
    using Shouldly;
    using Xunit;

    public class CommandLineOutput: Scenario
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
            // Adds the namespace "Cmd" to use ICommandLine
            // ## using Cmd;

            var lines = new System.Collections.Generic.List<string>();
            int? exitCode = GetService<ICommandLine>().Run(
                new CommandLine("cmd").AddArgs("/c", "SET").AddVars(("MyEnv", "MyVal")),
                i => lines.Add(i.Line));
            
            lines.ShouldContain("MyEnv=MyVal");
            // }
            
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}