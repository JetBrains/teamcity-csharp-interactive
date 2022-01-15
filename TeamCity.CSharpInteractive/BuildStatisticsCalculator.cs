// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;
using System.Linq;
using Dotnet;

internal class BuildStatisticsCalculator: IStatisticsCalculator
{
    public BuildStatistics Calculate(IReadOnlyCollection<Dotnet.BuildResult> results)
    {
        var testItems =
            from testGroup in
                from testResult in results.SelectMany(i => i.Tests)
                group testResult by (testResult.AssemblyName, testResult.DisplayName)
            select testGroup.OrderBy(i1 => i1.State).Last();
        
        var totalTests = 0;
        var failedTests = 0;
        var ignoredTests = 0;
        var passedTests = 0;
        foreach (var test in testItems)
        {
            totalTests++;
            switch (test.State)
            {
                case TestState.Passed:
                    passedTests++;
                    break;

                case TestState.Failed:
                    failedTests++;
                    break;

                case TestState.Ignored:
                    ignoredTests++;
                    break;
            }
        }

        return new BuildStatistics(
            string.Empty,
            results.SelectMany(i => i.Errors).Count(),
            results.SelectMany(i => i.Warnings).Count(),
            totalTests,
            failedTests,
            ignoredTests,
            passedTests);
    }
}