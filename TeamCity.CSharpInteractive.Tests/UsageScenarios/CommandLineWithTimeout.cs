// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using System.Threading;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class CommandLineWithTimeout: Scenario
    {
        [SkippableFact(Timeout = 5000)]
        public void Run()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

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