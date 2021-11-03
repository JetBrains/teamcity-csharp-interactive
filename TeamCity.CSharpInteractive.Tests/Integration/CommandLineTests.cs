// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.Integration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class CommandLineTests
    {
        [Fact]
        public void ShouldRun()
        {
            // Given
            var events = new List<CommandLineOutput>();

            // When
            var exitCode = Composer.ResolveICommandLine().Run(new CommandLine("whoami"), e => events.Add(e));

            // Then
            exitCode.HasValue.ShouldBeTrue();
            exitCode.ShouldBe(0);
            events.Any(i => i.IsError).ShouldBeFalse();
            events.Any(i => !i.IsError).ShouldBeTrue();
        }
        
        [Fact]
        public async Task ShouldRunAsync()
        {
            // Given
            var events = new List<CommandLineOutput>();

            // When
            var exitCode = await Composer.ResolveICommandLine().RunAsync(new CommandLine("whoami"), e => events.Add(e));

            // Then
            exitCode.HasValue.ShouldBeTrue();
            exitCode.ShouldBe(0);
            events.Any(i => i.IsError).ShouldBeFalse();
            events.Any(i => !i.IsError).ShouldBeTrue();
        }
    }
}