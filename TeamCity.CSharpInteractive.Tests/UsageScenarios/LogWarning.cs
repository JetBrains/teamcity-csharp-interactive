// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Xunit;

    public class LogWarning: Scenario
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
}