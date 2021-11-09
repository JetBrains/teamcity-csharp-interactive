// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class CommandLineSimple: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);

            // $visible=true
            // $tag=2 Command Line API
            // $priority=00
            // $description=Run
            // {
            var exitCode = GetService<ICommandLine>().Run(new CommandLine("whoami", "/all"));
            // }
            
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}