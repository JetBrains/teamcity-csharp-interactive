namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.IO;
    using Moq;
    using Shouldly;
    using Xunit;

    public class DotnetEnvironmentTests
    {
        private readonly Mock<IEnvironment> _env;

        public DotnetEnvironmentTests()
        {
            _env = new Mock<IEnvironment>();
        }
        
        [Fact]
        public void ShouldProvidePath()
        {
            // Given
            var instance = CreateInstance(".NETCoreApp,Version=v3.1");
            _env.Setup(i => i.GetPath(SpecialFolder.ProgramFiles)).Returns("Abc");

            // When
            var path = instance.Path;

            // Then
            path.ShouldBe(Path.Combine("Abc", "dotnet"));
        }

        [Fact]
        public void ShouldProvideTargetFrameworkMoniker()
        {
            // Given

            // When
            var instance = CreateInstance(".NETCoreApp,Version=v3.1");

            // Then
            instance.TargetFrameworkMoniker.ShouldBe(".NETCoreApp,Version=v3.1");
        }
        
        [Theory]
        [InlineData(".NETCoreApp,Version=v3.1", "3.1")]
        [InlineData(".NETCoreApp,Version=v5.0", "5.0")]
        public void ShouldProvideVersion(string targetFrameworkMoniker, string versionStr)
        {
            // Given
            var instance = CreateInstance(targetFrameworkMoniker);

            // When
            var version = instance.Version;

            // Then
            version.ShouldBe(Version.Parse(versionStr));
        }
        
        [Theory]
        [InlineData(".NETCoreApp,Version=v3.1", "netcoreapp3.1")]
        [InlineData(".NETCoreApp,Version=v5.0", "net5.0")]
        public void ShouldProvideTfm(string targetFrameworkMoniker, string expectedTfm)
        {
            // Given
            var instance = CreateInstance(targetFrameworkMoniker);

            // When
            var actualTfm = instance.Tfm;

            // Then
            actualTfm.ShouldBe(expectedTfm);
        }

        private DotnetEnvironment CreateInstance(string frameworkName) => 
            new(frameworkName, _env.Object);
    }
}