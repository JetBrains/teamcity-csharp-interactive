namespace TeamCity.CSharpInteractive.Tests
{
    using System.Linq;
    using Shouldly;
    using Xunit;

    public class HostIntegrationCodeSourceTests
    {
        [Fact]
        public void ShouldProvideSourceCode()
        {
            // Given
            var instance = CreateInstance();

            // When
            var actualCode = instance.ToList();

            // Then
            actualCode.ShouldBe(new []{"using NuGet;", "using static TeamCity.CSharpInteractive.Contracts.Color;"});
        }

        private static HostIntegrationCodeSource CreateInstance() => new();
    }
}