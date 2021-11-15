// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Xunit;

    public class LogError: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));
            
            // $visible=true
            // $tag=09 Build log API
            // $priority=02
            // $description=Log an error to a build log
            // {
            Error("Error info", "Error identifier");
            // }
        }
    }
}