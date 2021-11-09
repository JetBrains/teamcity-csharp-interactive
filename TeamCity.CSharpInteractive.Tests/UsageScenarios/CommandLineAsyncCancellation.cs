// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using System.Threading;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class CommandLineAsyncCancellation: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(System.Environment.OSVersion.Platform == PlatformID.Win32NT);

            // $visible=true
            // $tag=2 Command Line API
            // $priority=01
            // $description=Cancellation of asynchronous run
            // #header=The cancellation is finishing running process.
            // {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = GetService<ICommandLine>().RunAsync(new CommandLine("cmd", "TIMEOUT", "/T",  "120"), default, cancellationTokenSource.Token);
            
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));
            task.IsCompleted.ShouldBeFalse();
            // }
        }
    }
}