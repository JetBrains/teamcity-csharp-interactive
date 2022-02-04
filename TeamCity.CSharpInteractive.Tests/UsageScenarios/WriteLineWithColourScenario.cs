// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using static HostApi.Color;

public class WriteLineWithColourScenario : BaseScenario
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=09 Logging
        // $priority=01
        // $description=Write a line highlighted with "Header" color to a build log
        // {
        WriteLine("Hello", Header);
        // }
    }
}