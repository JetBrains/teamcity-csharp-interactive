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
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            // $visible=true
            // $tag=2 Command Line API
            // $priority=01
            // $description=Run a command line
            // {
            int? exitCode = GetService<ICommandLine>().Run(new CommandLine("whoami", "/all"));
            // }
            
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}