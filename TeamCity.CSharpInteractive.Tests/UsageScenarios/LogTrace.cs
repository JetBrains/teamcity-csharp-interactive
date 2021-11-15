// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Xunit;

    public class LogTrace: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));
            
            // $visible=true
            // $tag=09 Build log API
            // $priority=05
            // $description=Log trace information to a build log
            // {
            Trace("Some trace info");
            // }
        }
    }
}