// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System.Collections.Generic;
    using Cmd;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetRun: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=00
            // $description=Run a project
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Creates a new console project, running a command like: "dotnet new console -n MyApp --force"
            var result = build.Run(new Custom("new", "console", "-n", "MyApp", "--force"));
            result.State.ShouldBe(BuildState.Succeeded);

            // Runs the console project using a command like: "dotnet run" from the directory "MyApp"
            var stdOut = new List<string>(); 
            result = build.Run(new Run().WithWorkingDirectory("MyApp"), message => stdOut.Add(message.Text));
            result.State.ShouldBe(BuildState.Succeeded);
            
            // Checks StdOut
            stdOut.ShouldBe(new []{ "Hello, World!" });
            // }
        }
    }
}