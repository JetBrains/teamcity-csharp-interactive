// ReSharper disable RedundantUsingDirective
namespace TeamCity.CSharpInteractive.Tests.Integration;

using Core;
using HostApi;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
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
    }
}