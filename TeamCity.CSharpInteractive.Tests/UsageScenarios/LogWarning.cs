// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;

public class LogWarning : ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=09 Logging
        // $priority=03
        // $description=Log a warning to a build log
        // {
        Warning("Warning info");
        // }
    }
}