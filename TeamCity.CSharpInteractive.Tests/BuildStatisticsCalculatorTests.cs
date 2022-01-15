namespace TeamCity.CSharpInteractive.Tests;

using System.Collections.Generic;
using Dotnet;
using Shouldly;
using Xunit;

public class BuildStatisticsCalculatorTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldCalculate(BuildResult[] results, BuildStatistics expected)
    {
        // Given
        var calculator = CreateInstance();

        // When
        var actual = calculator.Calculate(results);

        // Then
        actual.ShouldBe(expected);
    }
    
    public static IEnumerable<object[]> Data => new List<object[]>
    {
        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddErrors(new BuildMessage(BuildMessageState.Error), new BuildMessage(BuildMessageState.Error)).AddWarnings(new BuildMessage(BuildMessageState.Warning))
            },
            BuildStatistics.Empty.WithErrors(2).WithWarnings(1)
        },

        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddErrors(new BuildMessage(BuildMessageState.Error), new BuildMessage(BuildMessageState.Error)).AddWarnings(new BuildMessage(BuildMessageState.Warning)),
                BuildResult.Succeeded.AddErrors(new BuildMessage(BuildMessageState.Error)).AddWarnings(new BuildMessage(BuildMessageState.Warning))
            },
            BuildStatistics.Empty.WithErrors(3).WithWarnings(2)
        },

        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Failed, "Test2").WithAssemblyName("Assembly1"))
            },
            BuildStatistics.Empty.WithTests(2).WithFailedTests(2)
        },
        
        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly2"))
            },
            BuildStatistics.Empty.WithTests(2).WithFailedTests(2)
        },
        
        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"))
            },
            BuildStatistics.Empty.WithTests(1).WithFailedTests(1)
        },
        
        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Ignored, "Test2").WithAssemblyName("Assembly1"), new TestResult(TestState.Passed, "Test3").WithAssemblyName("Assembly1"))
            },
            BuildStatistics.Empty.WithTests(3).WithFailedTests(1).WithIgnoredTests(1).WithPassedTests(1)
        },
        
        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Ignored, "Test2").WithAssemblyName("Assembly1"), new TestResult(TestState.Passed, "Test3").WithAssemblyName("Assembly1")),
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Ignored, "Test2").WithAssemblyName("Assembly1"), new TestResult(TestState.Passed, "Test3").WithAssemblyName("Assembly1"))
            },
            BuildStatistics.Empty.WithTests(3).WithFailedTests(1).WithIgnoredTests(1).WithPassedTests(1)
        },
        
        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Ignored, "Test2").WithAssemblyName("Assembly1"), new TestResult(TestState.Passed, "Test3").WithAssemblyName("Assembly1")),
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly2"), new TestResult(TestState.Ignored, "Test2").WithAssemblyName("Assembly2"), new TestResult(TestState.Passed, "Test3").WithAssemblyName("Assembly1"))
            },
            BuildStatistics.Empty.WithTests(5).WithFailedTests(2).WithIgnoredTests(2).WithPassedTests(1)
        },
        
        new object[]
        {
            new []
            {
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly1"), new TestResult(TestState.Ignored, "Test2").WithAssemblyName("Assembly1"), new TestResult(TestState.Passed, "Test3").WithAssemblyName("Assembly1"))
                    .AddErrors(new BuildMessage(BuildMessageState.Error), new BuildMessage(BuildMessageState.Error)).AddWarnings(new BuildMessage(BuildMessageState.Warning)),
                BuildResult.Succeeded.AddTests(new TestResult(TestState.Failed, "Test1").WithAssemblyName("Assembly2"), new TestResult(TestState.Ignored, "Test2").WithAssemblyName("Assembly2"), new TestResult(TestState.Passed, "Test3").WithAssemblyName("Assembly1"))
            },
            BuildStatistics.Empty.WithTests(5).WithFailedTests(2).WithIgnoredTests(2).WithPassedTests(1).WithErrors(2).WithWarnings(1)
        }
    };

    private static BuildStatisticsCalculator CreateInstance() =>
        new();
}