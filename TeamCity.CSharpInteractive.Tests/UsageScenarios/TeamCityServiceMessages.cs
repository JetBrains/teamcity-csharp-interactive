// ReSharper disable StringLiteralTypo
// ReSharper disable ConvertToUsingDeclaration
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

public class TeamCityServiceMessages : ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        Skip.IfNot(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")));

        // $visible=true
        // $tag=3 TeamCity Service Messages API
        // $priority=00
        // $description=TeamCity integration via service messages
        // $header=For more details how to use TeamCity service message API please see [this](https://github.com/JetBrains/TeamCity.ServiceMessages) page. Instead of creating a root message writer like in the following example:
        // $header=``` CSharp
        // $header=using JetBrains.TeamCity.ServiceMessages.Write.Special;
        // $header=using var writer = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
        // $header=```
        // $header=use this statement:
        // $header=``` CSharp
        // $header=using JetBrains.TeamCity.ServiceMessages.Write.Special;
        // $header=using var writer = GetService<ITeamCityWriter>();
        // $header=```
        // $header=This sample opens a block _My Tests_ and reports about two tests:
        // $footer=For more information on TeamCity Service Messages, see [this](https://www.jetbrains.com/help/teamcity/service-messages.html) page.
        // {
        // Adds a namespace to use ITeamCityWriter
        // ## using JetBrains.TeamCity.ServiceMessages.Write.Special;

        using var writer = GetService<ITeamCityWriter>();
        using (var tests = writer.OpenBlock("My Tests"))
        {
            using (var test = tests.OpenTest("Test1"))
            {
                test.WriteStdOutput("Hello");
                test.WriteImage("TestsResults/Test1Screenshot.jpg", "Screenshot");
                test.WriteDuration(TimeSpan.FromMilliseconds(10));
            }

            using (var test = tests.OpenTest("Test2"))
            {
                test.WriteIgnored("Some reason");
            }
        }
        // }
    }
}