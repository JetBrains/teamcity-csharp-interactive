// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Shouldly;
    using Xunit;
    
    public class CommandLineAsync: Scenario
    {
        [SkippableFact]
        public async Task Run()
        {
            Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);

            // $visible=true
            // $tag=2 Command Line API
            // $priority=01
            // $description=Run asynchronously
            // {
            var exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
            // }
            
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}