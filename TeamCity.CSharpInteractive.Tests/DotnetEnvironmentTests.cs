namespace TeamCity.CSharpInteractive.Tests
{
    using Shouldly;
    using Xunit;

    public class DotnetEnvironmentTests
    {
        [Fact]
        public void ShouldProvideTargetFrameworkMoniker()
        {
            // Given

            // When
            var instance = CreateInstance(".NETCoreApp,Version=v3.1");

            // Then
            instance.TargetFrameworkMoniker.ShouldBe(".NETCoreApp,Version=v3.1");
        }
        
        private static DotnetEnvironment CreateInstance(string frameworkName) => 
            new(frameworkName);
    }
}