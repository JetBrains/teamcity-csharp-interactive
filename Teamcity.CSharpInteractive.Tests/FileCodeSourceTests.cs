namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class FileCodeSourceTests
    {
        [Fact]
        public void ShouldProvideFileContent()
        {
            // Given
            var reader = new Mock<IFileTextReader>();
            reader.Setup(i => i.Read("Abc")).Returns("content");
            var source = new FileCodeSource(Mock.Of<ILog<FileCodeSource>>(), reader.Object)
            {
                FileName = "Abc"
            };

            // When
            var expectedResult = source.ToArray();

            // Then
            expectedResult.ShouldBe(new []{"content"});
        }
        
        [Fact]
        public void ShouldLogErrorWhenReadingFailed()
        {
            // Given
            var error = new Exception("test");
            var reader = new Mock<IFileTextReader>();
            reader.Setup(i => i.Read("Abc")).Throws(error);
            var log = new Mock<ILog<FileCodeSource>>();
            var source = new FileCodeSource(log.Object, reader.Object)
            {
                FileName = "Abc"
            };

            // When
            var expectedResult = source.ToArray();

            // Then
            expectedResult.ShouldBeEmpty();
            log.Verify(i => i.Error(It.IsAny<Text[]>()));
        }
    }
}