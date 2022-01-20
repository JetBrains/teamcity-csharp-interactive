namespace TeamCity.CSharpInteractive.Tests.ContractTests;

using System.Collections.Generic;
using System.Linq;
using Cmd;
using DotNet;
using Moq;
using Shouldly;
using Xunit;

public class BuildResultTests
{
    [Theory]
    [InlineData("Abc", 0, 2, 3, 2, 3, 4, "\"Abc\" is finished with 2 errors, 3 warnings, 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", null, 2, 3, 2, 3, 4, "\"Abc\" is not finished with 2 errors, 3 warnings, 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 1, 1, 1, 1, 1, 1, "\"Abc\" is finished with 1 error, 1 warning, 1 failed, 1 ignored, 1 passed and 3 total tests.")]
    [InlineData("Abc", 2, 0, 3, 2, 3, 4, "\"Abc\" is finished with 3 warnings, 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 3, 2, 0, 2, 3, 4, "\"Abc\" is finished with 2 errors, 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 4, 0, 0, 2, 3, 4, "\"Abc\" is finished with 2 failed, 3 ignored, 4 passed and 9 total tests.")]
    [InlineData("Abc", 5, 0, 0, 1, 0, 0, "\"Abc\" is finished with 1 failed and 1 total test.")]
    [InlineData("", 6 ,0, 0, 1, 0, 0, "Build is finished with 1 failed and 1 total test.")]
    [InlineData("", null ,0, 0, 0, 0, 0, "Build is not finished.")]
    [InlineData("Abc", null ,0, 0, 0, 0, 0, "\"Abc\" is not finished.")]
    public void ShouldSupportToString(string shortName, int? exitCode, int errors, int warnings, int failedTests, int ignoredTests, int passedTests, string expected)
    {
        // Given
        var startInfo = new Mock<IStartInfo>();
        startInfo.SetupGet(i => i.ShortName).Returns(shortName);
        var result = 
            new BuildResult(startInfo.Object)
                .WithExitCode(exitCode)
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