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
        [SkippableFact(Timeout = 5000)]
        public async Task Run()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            // $visible=true
            // $tag=2 Command Line API
            // $priority=02
            // $description=Run a command line asynchronously
            // {
            int? exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
            // }
            
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}