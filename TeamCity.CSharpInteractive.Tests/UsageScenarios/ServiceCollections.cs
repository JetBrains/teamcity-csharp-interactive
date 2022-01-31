// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

public class ServiceCollections : ScenarioHostService
{
    [Fact]
    // $visible=true
    // $tag=08 Global state
    // $priority=03
    // $description=Service collection
    // {
    public void Run()
    {
        var serviceProvider = 
            GetService<IServiceCollection>()
            .AddTransient<MyTask>()
            .BuildServiceProvider();

        var myTask = serviceProvider.GetRequiredService<MyTask>();
        var exitCode = myTask.Run();
        exitCode.ShouldBe(0);
    }

    class MyTask
    {
        private readonly ICommandLineRunner _runner;

        public MyTask(ICommandLineRunner runner) => 
            _runner = runner;

        public int? Run() => 
            _runner.Run(new CommandLine("whoami"));
    }
    
    // }
}