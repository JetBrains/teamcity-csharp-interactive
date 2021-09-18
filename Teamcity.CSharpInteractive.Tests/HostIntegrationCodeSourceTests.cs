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
            actualCode.ShouldBe(new []{HostIntegrationCodeSource.UsingStatic});
        }

        private static HostIntegrationCodeSource CreateInstance() => new();
    }
}