// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

public class Props : ScenarioHostService
{
    [Fact]
    public void Run()
    {
        // $visible=true
        // $tag=08 Global state
        // $priority=01
        // $description=Using Props dictionary
        // $header=Properties _Props_ have got from TeamCity system properties automatically.
        // {
        WriteLine(Props["TEAMCITY_VERSION"]);
        WriteLine(Props["TEAMCITY_PROJECT_NAME"]);

        // This property will be available at the next TeamCity steps as system parameter _system.Version_
        // and some runners, for instance, the .NET runner, pass it as a build property.
        Props["Version"] = "1.1.6";
        // }
    }
}