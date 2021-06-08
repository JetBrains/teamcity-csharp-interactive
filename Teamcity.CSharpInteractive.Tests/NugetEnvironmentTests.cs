namespace Teamcity.CSharpInteractive.Tests
{
    using System.IO;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class NugetEnvironmentTests
    {
        private readonly Mock<IDotnetEnvironment> _dotnetEnvironment;

        public NugetEnvironmentTests()
        {
            _dotnetEnvironment = new Mock<IDotnetEnvironment>();
        }

        [Fact]
        public void ShouldProvideFallbackFolders()
        {
            // Given
            var instance = CreateInstance();
            _dotnetEnvironment.SetupGet(i => i.Path).Returns("Abc");

            // When
            var actualFallbackFolders = instance.FallbackFolders.ToArray();

            // Then
            actualFallbackFolders.ShouldBe(new []{Path.Combine("Abc", "sdk", "NuGetFallbackFolder")});
        }
        
        [Fact]
        public void ShouldProvideSources()
        {
            // Given
            var instance = CreateInstance();
            _dotnetEnvironment.SetupGet(i => i.Path).Returns("Abc");

            // When
            var actualSources = instance.Sources.ToArray();

            // Then
            actualSources.ShouldBe(new []{@"https://api.nuget.org/v3/index.json"});
        }

        private NugetEnvironment CreateInstance() =>
            new(_dotnetEnvironment.Object);
    }
}