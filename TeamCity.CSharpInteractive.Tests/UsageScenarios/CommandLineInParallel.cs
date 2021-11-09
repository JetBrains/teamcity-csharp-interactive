// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class CommandLineInParallel: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);

            // $visible=true
            // $tag=2 Command Line API
            // $priority=04
            // $description=Run asynchronously in parallel
            // {
            var task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
            var exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
            task.Wait();
            // }
            
            task.Result.HasValue.ShouldBeTrue();
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}