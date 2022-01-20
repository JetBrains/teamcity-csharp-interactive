// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

public class LogInfo: Scenario
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));
            
        // $visible=true
        // $tag=09 Logging
        // $priority=04
        // $description=Log information to a build log
        // {
        Info("Some info");
        // }
    }
}