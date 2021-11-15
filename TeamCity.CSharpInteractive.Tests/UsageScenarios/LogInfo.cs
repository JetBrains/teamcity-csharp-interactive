// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Xunit;

    public class LogInfo: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));
            
            // $visible=true
            // $tag=09 Build log API
            // $priority=04
            // $description=Log information to a build log
            // {
            Info("Some info");
            // }
        }
    }
}