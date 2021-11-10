namespace TeamCity.CSharpInteractive.Tests.Integration
{
    using System.Linq;
    using Contracts;
    using Core;
    using Shouldly;
    using Xunit;

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

            // When
            var result = TestTool.Run(DotNetScript.Shared.AddArgs(arg).AddVars(TestTool.DefaultVars));
            
            // Then
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Any(i => i.StartsWith("Usage:")).ShouldBeTrue();
            result.StdOut.Any(i => i.StartsWith("Options:")).ShouldBeTrue();
        }
    }
}