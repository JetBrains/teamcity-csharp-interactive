// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

public class LogError: ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));
            
        // $visible=true
        // $tag=09 Logging
        // $priority=02
        // $description=Log an error to a build log
        // {
        Error("Error info", "Error identifier");
        // }
    }
}