// ReSharper disable StringLiteralTypo
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Dotnet;
    using Shouldly;
    using Xunit;

    public class DotnetCustom: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=11 .NET build API
            // $priority=00
            // $description=Run a custom .NET command
            // {
            // Adds the namespace "Dotnet" to use .NET build API
            // ## using Dotnet;

            // Resolves a build service
            var build = GetService<IBuild>();
            
            // Gets the dotnet version, running a command like: "dotnet --version"
            Version? version = default;
            var result = build.Run(
                new Custom("--version"),
                output => Version.TryParse(output.Line, out version));

            result.Success.ShouldBeTrue();
            version.ShouldNotBeNull();
            // }
        }
    }
}