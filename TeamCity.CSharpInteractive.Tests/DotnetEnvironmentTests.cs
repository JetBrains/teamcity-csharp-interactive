namespace TeamCity.CSharpInteractive.Tests
{
    using Moq;
    using Shouldly;
    using Xunit;

    public class DotnetEnvironmentTests
    {
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<IFileExplorer> _fileExplorer;

        public DotnetEnvironmentTests()
        {
            _environment = new Mock<IEnvironment>();
            _fileExplorer = new Mock<IFileExplorer>();
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
            new(frameworkName, _environment.Object, _fileExplorer.Object);
    }
}