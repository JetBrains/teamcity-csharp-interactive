namespace TeamCity.CSharpInteractive.Tests
{
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
        
        private DotnetEnvironment CreateInstance(string frameworkName) => 
            new(frameworkName, _env.Object);
    }
}