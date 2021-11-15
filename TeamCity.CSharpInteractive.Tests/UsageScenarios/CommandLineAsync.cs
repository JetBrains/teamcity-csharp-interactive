// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
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
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);
            Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

            // $visible=true
            // $tag=10 Command Line API
            // $priority=02
            // $description=Run a command line asynchronously
            // {
            int? exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
            // }
            
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}