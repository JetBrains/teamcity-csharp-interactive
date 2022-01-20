// ReSharper disable RedundantUsingDirective
namespace TeamCity.CSharpInteractive.Tests.Integration;

using Contracts;
using Core;
using Cmd;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class HelpTests
{
    [Theory]
    [InlineData("/?")]
    [InlineData("--Help")]
    [InlineData("--help")]
    [InlineData("/help")]
    [InlineData("-h")]
    [InlineData("/h")]
    public void ShouldShowHelp(string arg)
    {
        // Given
        var cmd = DotNetScript.Create().AddArgs(arg).AddVars(TestTool.DefaultVars);

        // When
        var result = TestTool.Run(cmd);
            
        // Then
        result.ExitCode.ShouldBe(0);
        result.StdErr.ShouldBeEmpty();
        result.StdOut.Any(i => i.StartsWith("Usage:")).ShouldBeTrue();
        result.StdOut.Any(i => i.StartsWith("Options:")).ShouldBeTrue();
    }
}