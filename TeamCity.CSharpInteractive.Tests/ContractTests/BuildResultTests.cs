namespace TeamCity.CSharpInteractive.Tests.ContractTests;

using System.Collections.Generic;
using System.Linq;
using Cmd;
using Dotnet;
using Moq;
using Shouldly;
using Xunit;

public class BuildResultTests
{
    [Theory]
    [InlineData("Abc", 2, 3, 2, 3, 4, "\"Abc\" is succeeded with 2 errors, 3 warnings, 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 1, 1, 1, 1, 1, "\"Abc\" is succeeded with 1 error, 1 warning, 1 failed, 1 ignored, 1 passed and 3 total tests.")]
    [InlineData("Abc", 0, 3, 2, 3, 4, "\"Abc\" is succeeded with 3 warnings, 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 2, 0, 2, 3, 4, "\"Abc\" is succeeded with 2 errors, 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 0, 0, 2, 3, 4, "\"Abc\" is succeeded with 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 0, 0, 1, 0, 0, "\"Abc\" is succeeded with 1 failed and 1 total test.")]
    [InlineData("", 0, 0, 1, 0, 0, "Build is succeeded with 1 failed and 1 total test.")]
    public void ShouldSupportToString(string shortName, int errors, int warnings, int failedTests, int ignoredTests, int passedTests, string expected)
    {
        // Given
        var startInfo = new Mock<IStartInfo>();
        startInfo.SetupGet(i => i.ShortName).Returns(shortName);
        var result = 
            new BuildResult(BuildState.Succeeded, startInfo.Object)
                .WithErrors(Enumerable.Repeat(new BuildMessage(BuildMessageState.Error), errors).ToArray())
                .WithWarnings(Enumerable.Repeat(new BuildMessage(BuildMessageState.Warning), warnings).ToArray())
                .WithTests(
                    GetTests(TestState.Failed, failedTests)
                        .Concat(GetTests(TestState.Ignored, ignoredTests))
                        .Concat(GetTests(TestState.Passed, passedTests))
                        .ToArray());
        
        // When
        var actual = result.ToString();

        // Then
        actual.ShouldBe(expected);
    }

    private static IEnumerable<TestResult> GetTests(TestState state, int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return new TestResult(state, $"Test_{state}_{i}").WithAssemblyName("aaa");
        }
    }
}