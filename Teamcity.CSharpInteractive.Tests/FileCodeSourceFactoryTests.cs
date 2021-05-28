namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Shouldly;
    using Xunit;

    public class FileCodeSourceFactoryTests
    {
        [Fact]
        public void ShouldCreateSource()
        {
            // Given
            var expectedSource = new FileCodeSource(Mock.Of<ILog<FileCodeSource>>(), Mock.Of<IFileTextReader>()) {FileName = "Abc"};
            var factory = new FileCodeSourceFactory(() => new FileCodeSource(Mock.Of<ILog<FileCodeSource>>(), Mock.Of<IFileTextReader>()));
            
            // When
            var source = factory.Create("Abc");

            // Then
            source.Name.ShouldBe("Abc");
        }
    }
}