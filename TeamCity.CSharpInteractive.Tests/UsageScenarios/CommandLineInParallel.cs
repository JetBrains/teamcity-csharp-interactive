// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using System.Threading.Tasks;
    using Cmd;
    using Shouldly;
    using Xunit;

    public class CommandLineInParallel: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);
            Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

            // $visible=true
            // $tag=10 Command Line API
            // $priority=05
            // $description=Run asynchronously in parallel
            // {
            // Adds the namespace "Cmd" to use ICommandLine
            // ## using Cmd;

            Task<int?> task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
            int? exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
            task.Wait();
            // }
            
            task.Result.HasValue.ShouldBeTrue();
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}