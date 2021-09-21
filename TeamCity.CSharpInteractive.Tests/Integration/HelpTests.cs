namespace TeamCity.CSharpInteractive.Tests.Integration
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Core;
    using Shouldly;
    using Xunit;

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
            var runner = Composer.Resolve<IProcessRunner>();
            
            // When
            var result = runner.Run(new []{new CommandLineArgument(arg)}, TestTool.DefaultVars);
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Any(i => new Regex($"^{Info.Header} [\\d\\.]+ \\.NETCoreApp,Version=v.+$").IsMatch(i)).ShouldBeTrue();
            result.StdOut.Any(i => i.StartsWith("Usage:")).ShouldBeTrue();
            result.StdOut.Any(i => i.StartsWith("Options:")).ShouldBeTrue();
        }
    }
}