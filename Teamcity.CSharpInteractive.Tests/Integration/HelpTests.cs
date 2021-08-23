namespace Teamcity.CSharpInteractive.Tests.Integration
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
            var result = runner.Run(arg);
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Any(i => new Regex($"^{Info.Header} [\\d\\.]+ net.+$").IsMatch(i)).ShouldBeTrue();
            result.StdOut.Any(i => i.StartsWith("Usage:")).ShouldBeTrue();
            result.StdOut.Any(i => i.StartsWith("Options:")).ShouldBeTrue();
        }
    }
}