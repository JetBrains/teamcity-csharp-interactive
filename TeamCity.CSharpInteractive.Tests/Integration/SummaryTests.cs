// ReSharper disable StringLiteralTypo
// ReSharper disable RedundantUsingDirective
namespace TeamCity.CSharpInteractive.Tests.Integration;

using System;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class SummaryTests
{
    [Fact]
    public void ShouldInterpretAsSucceededWhenHasStdErr()
    {
        // Given

        // When
        var result = TestTool.Run("Console.Error.WriteLine(\"StdErr\");");

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdOut.Contains("Running succeeded.").ShouldBeTrue(result.ToString());
        result.StdOut.Contains("StdErr").ShouldBeFalse(result.ToString());
        result.StdErr.Contains("StdErr").ShouldBeTrue(result.ToString());
    }
    
    [Fact]
    public void ShouldInterpretAsFailedWhenError()
    {
        // Given

        // When
        var result = TestTool.Run("Error(\"StdErr\");");

        // Then
        result.ExitCode.ShouldBe(1, result.ToString());
        result.StdOut.Contains("Running FAILED.").ShouldBeTrue(result.ToString());
        result.StdOut.Any(i => i.Contains("StdErr")).ShouldBeTrue(result.ToString());
        result.StdErr.Contains("StdErr").ShouldBeFalse(result.ToString());
    }
}