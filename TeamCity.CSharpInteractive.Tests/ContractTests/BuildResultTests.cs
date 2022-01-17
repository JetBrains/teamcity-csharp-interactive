namespace TeamCity.CSharpInteractive.Tests.ContractTests;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cmd;
using Dotnet;
using Moq;
using Shouldly;
using Xunit;

public class BuildResultTests
{
    private static readonly Mock<IStatisticsCalculator> StatisticsCalculator = new();
    
    [Theory]
    [MemberData(nameof(StateData))]
    public void ShouldProvideCompositeState(BuildState initialState, IReadOnlyList<BuildResult> results, BuildState expectedState)
    {
        // Given
        var result = new BuildResult(initialState).WithResults(results);

        // When
        var actualState = result.State;

        // Then
        actualState.ShouldBe(expectedState);
    }

    public static IEnumerable<object?[]> StateData => new List<object?[]>
    {
        new object[]
        {
            BuildState.Succeeded,
            new [] {BuildResult.Succeeded, BuildResult.Succeeded},
            BuildState.Succeeded
        },
        
        new object[]
        {
            BuildState.Canceled,
            new [] {BuildResult.Succeeded, BuildResult.Succeeded},
            BuildState.Canceled
        },
        
        new object[]
        {
            BuildState.Failed,
            new [] {BuildResult.Succeeded, BuildResult.Succeeded},
            BuildState.Failed
        },
        
        new object[]
        {
            BuildState.Succeeded,
            new [] {BuildResult.Failed, BuildResult.Succeeded},
            BuildState.Failed
        },
        
        new object[]
        {
            BuildState.Succeeded,
            new [] {BuildResult.Failed, BuildResult.Canceled},
            BuildState.Canceled
        },
        
        new object[]
        {
            BuildState.Canceled,
            new [] {BuildResult.Failed, BuildResult.Succeeded},
            BuildState.Canceled
        }
    };
    
    [Fact]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public void ShouldProvideSummary()
    {
        // Given
        var statisticsCalculator = new Mock<IStatisticsCalculator>();
        statisticsCalculator.Setup(i => i.Calculate(It.IsAny<BuildResult[]>())).Returns(BuildStatistics.Empty);
        var result = BuildResult.Succeeded.AddResults(BuildResult.Failed).WithStatisticsCalculator(statisticsCalculator.Object);
        
        // When
        var summary = result.Summary;
        var summary2 = result.Summary;

        // Then
        statisticsCalculator.Verify(i => i.Calculate(new []{ result }), Times.Once);
    }
    
    [Fact]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public void ShouldProvideTotals()
    {
        // Given
        StatisticsCalculator.Reset();
        StatisticsCalculator.Setup(i => i.Calculate(It.IsAny<BuildResult[]>())).Returns(BuildStatistics.Empty);
        var result2 = BuildResult.Canceled;
        var result3 = BuildResult.Failed.WithResults(result2);
        var result = BuildResult.Succeeded.AddResults(result2, result3, result2).WithStatisticsCalculator(StatisticsCalculator.Object);
        
        // When
        var summary = result.Totals;
        var summary2 = result.Totals;

        // Then
        StatisticsCalculator.Verify(i => i.Calculate(It.Is<IReadOnlyCollection<BuildResult>>(results => results.SequenceEqual(new [] {result2, result3, result}))), Times.Once);
    }
    
    [Fact]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public void ShouldConvertToCompositeResult()
    {
        // Given
        var result1 = BuildResult.Canceled.WithErrors(new BuildMessage(BuildMessageState.Warning));
        var result2 = BuildResult.Failed.WithResults(result1).WithTests(new TestResult(TestState.Failed, "Test1"));
        var result3 = BuildResult.Succeeded.AddResults(result1, result2, result1).WithErrors(new BuildMessage(BuildMessageState.Error));

        // When
        BuildResult result = new [] { result1, result2, result1, result3 };

        // Then
        result.Results.ShouldBe(new []{ result1, result2, result3 });
        result.Errors.ShouldBeEmpty();
        result.Warnings.ShouldBeEmpty();
        result.Tests.ShouldBeEmpty();
        result.CommandLines.ShouldBeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(ToStringData))]
    public void ShouldSupportToString(BuildResult result, BuildResult[] additionalResults, BuildStatistics summary, BuildStatistics totals, string expected)
    {
        // Given
        result = result.WithResults(additionalResults);
        StatisticsCalculator.Reset();
        StatisticsCalculator.Setup(i => i.Calculate(It.IsAny<IReadOnlyCollection<BuildResult>>())).Returns(BuildStatistics.Empty);
        StatisticsCalculator.Setup(i => i.Calculate(It.Is<IReadOnlyCollection<BuildResult>>(results => results.SequenceEqual(new []{result})))).Returns(summary);
        if(additionalResults.Any())
        {
            StatisticsCalculator.Setup(i => i.Calculate(It.Is<IReadOnlyCollection<BuildResult>>(results => results.SequenceEqual(additionalResults.Concat(new []{ result}))))).Returns(totals);
        }

        // When
        var actual = result.ToString();

        // Then
        actual.ShouldBe(expected);
    }

    public static IEnumerable<object?[]> ToStringData()
    {
        // Empty summary and totals, no results, no command lines
        yield return new object[]
        {
            BuildResult.Succeeded,
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty,
            BuildStatistics.Empty,
            "This build is succeeded."
        };

        yield return new object[]
        {
            BuildResult.Failed,
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty,
            BuildStatistics.Empty,
            "This build is failed."
        };

        yield return new object[]
        {
            BuildResult.Canceled,
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty,
            BuildStatistics.Empty,
            "This build is canceled."
        };

        // Empty summary and totals, no results, has command lines
        var startInfo1 = new Mock<IStartInfo>();
        startInfo1.SetupGet(i => i.ShortName).Returns("Cmd 1");
        var cmd1 = new CommandLineResult(startInfo1.Object, 1);
        
        var startInfo2 = new Mock<IStartInfo>();
        startInfo2.SetupGet(i => i.ShortName).Returns("Cmd 2");
        var cmd2 = new CommandLineResult(startInfo2.Object, 1);
        
        yield return new object[]
        {
            BuildResult.Succeeded.WithCommandLines(cmd1),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty,
            BuildStatistics.Empty,
            "\"Cmd 1\" is succeeded."
        };
        
        yield return new object[]
        {
            BuildResult.Succeeded.WithCommandLines(cmd1, cmd1),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty,
            BuildStatistics.Empty,
            "\"Cmd 1\" is succeeded."
        };
        
        yield return new object[]
        {
            BuildResult.Succeeded.WithCommandLines(cmd1, cmd2),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty,
            BuildStatistics.Empty,
            "\"Cmd 1\", \"Cmd 2\" are succeeded."
        };
        
        yield return new object[]
        {
            BuildResult.Succeeded.WithCommandLines(cmd1, cmd1, cmd2),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty,
            BuildStatistics.Empty,
            "\"Cmd 1\", \"Cmd 2\" are succeeded."
        };

        // Has summary and has no totals, no results
        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty.WithErrors(1),
            BuildStatistics.Empty,
            "This build is succeeded with 1 error."
        };

        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty.WithErrors(2),
            BuildStatistics.Empty,
            "This build is succeeded with 2 errors."
        };

        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty.WithWarnings(1),
            BuildStatistics.Empty,
            "This build is succeeded with 1 warning."
        };

        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty.WithErrors(3).WithWarnings(2),
            BuildStatistics.Empty,
            "This build is succeeded with 3 errors and 2 warnings."
        };
        
        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object).WithCommandLines(cmd1, cmd2),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty.WithErrors(3).WithWarnings(2),
            BuildStatistics.Empty,
            "\"Cmd 1\", \"Cmd 2\" are succeeded with 3 errors and 2 warnings."
        };
        
        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object).WithCommandLines(cmd1, cmd2),
            Array.Empty<BuildResult>(),
            BuildStatistics.Empty.WithErrors(1).WithWarnings(2),
            BuildStatistics.Empty,
            "\"Cmd 1\", \"Cmd 2\" are succeeded with 1 error and 2 warnings."
        };
        
        // Has summary and has totals
        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object).WithCommandLines(cmd1, cmd2),
            new [] { BuildResult.Failed },
            BuildStatistics.Empty.WithErrors(3).WithWarnings(2),
            BuildStatistics.Empty.WithErrors(7).WithWarnings(5),
            "\"Cmd 1\", \"Cmd 2\", including 1 build before are failed with 3 errors and 2 warnings and with 7 total errors and 5 total warnings."
        };
        
        yield return new object[]
        {
            BuildResult.Succeeded.WithStatisticsCalculator(StatisticsCalculator.Object).WithCommandLines(cmd1, cmd2),
            new [] { BuildResult.Succeeded, BuildResult.Failed },
            BuildStatistics.Empty.WithErrors(3).WithWarnings(2),
            BuildStatistics.Empty.WithErrors(7).WithWarnings(5),
            "\"Cmd 1\", \"Cmd 2\", including 2 builds before are failed with 3 errors and 2 warnings and with 7 total errors and 5 total warnings."
        };
        
        yield return new object[]
        {
            BuildResult.Failed.WithStatisticsCalculator(StatisticsCalculator.Object).WithCommandLines(cmd1, cmd2),
            new [] { BuildResult.Succeeded, BuildResult.Canceled },
            BuildStatistics.Empty.WithErrors(3).WithWarnings(2),
            BuildStatistics.Empty.WithErrors(7).WithWarnings(5),
            "\"Cmd 1\", \"Cmd 2\", including 2 builds before are canceled with 3 errors and 2 warnings and with 7 total errors and 5 total warnings."
        };
        
        yield return new object[]
        {
            BuildResult.Failed.WithStatisticsCalculator(StatisticsCalculator.Object).WithCommandLines(cmd1, cmd2),
            new [] { BuildResult.Succeeded, BuildResult.Canceled },
            BuildStatistics.Empty.WithErrors(3).WithWarnings(2).WithTests(2).WithIgnoredTests(2).WithFailedTests(1),
            BuildStatistics.Empty.WithErrors(7).WithWarnings(5).WithTests(4).WithPassedTests(6).WithFailedTests(3),
            "\"Cmd 1\", \"Cmd 2\", including 2 builds before are canceled with 3 errors, 2 warnings, 1 failed, 2 ignored and 2 tests and with 7 total errors, 5 total warnings, 3 total failed, 6 total passed and 4 total tests."
        };
    }
}