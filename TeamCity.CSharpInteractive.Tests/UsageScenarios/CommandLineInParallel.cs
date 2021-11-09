// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class CommandLineInParallel: Scenario
    {
        [SkippableFact(Timeout = 5000)]
        public void Run()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            // $visible=true
            // $tag=2 Command Line API
            // $priority=05
            // $description=Run asynchronously in parallel
            // {
            Task<int?> task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
            int? exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
            task.Wait();
            // }
            
            task.Result.HasValue.ShouldBeTrue();
            exitCode.HasValue.ShouldBeTrue();
        }
    }
}