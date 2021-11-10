// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class CommandLineWithTimeout: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);
            Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

            // $visible=true
            // $tag=2 Command Line API
            // $priority=06
            // $description=Run timeout
            // $header=If timeout expired a process will be killed.
            // {
            int? exitCode = GetService<ICommandLine>().Run(
                new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
                default,
                TimeSpan.FromMilliseconds(1));
            
            exitCode.HasValue.ShouldBeFalse();
            // }
        }
    }
}