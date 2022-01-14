// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using Xunit;

    public class WriteEmptyLine: Scenario
    {
        [Fact]
        public void Run()
        {
            // $visible=true
            // $tag=09 Logging
            // $priority=01
            // $description=Write an empty line to a build log
            // {
            WriteLine();
            // }
        }
    }
}