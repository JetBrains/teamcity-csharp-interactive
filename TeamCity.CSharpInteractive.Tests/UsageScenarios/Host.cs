// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable UnusedVariable
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using NuGet;
    using Xunit;

    public class Host: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=08 Global state
            // $priority=02
            // $description=Using the _Host_ property
            // $header=[_Host_](TeamCity.CSharpInteractive.Contracts/IHost.cs) is actually the provider of all global properties and methods.
            // {
            var packages = Host.GetService<INuGet>();
            Host.WriteLine("Hello");
            // }
        }
    }
}