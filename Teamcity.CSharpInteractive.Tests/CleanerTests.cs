namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Xunit;

    public class CleanerTests
    {
        [Fact]
        public void ShouldTrack()
        {
            // Given
            var fs = new Mock<IFileSystem>();
            var cleaner = new Cleaner(Mock.Of<ILog<Cleaner>>(), fs.Object);
            
            // When
            cleaner.Track("Abc.txt");
            
            // Then
            fs.Verify(i => i.DeleteDirectory(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }
        
        [Fact]
        public void ShouldDeleteDirectoryOnTrackFinish()
        {
            // Given
            var fs = new Mock<IFileSystem>();
            var cleaner = new Cleaner(Mock.Of<ILog<Cleaner>>(), fs.Object);
            var cleanerToken = cleaner.Track("Abc.txt");

            // When
            cleanerToken.Dispose();

            // Then
            fs.Verify(i => i.DeleteDirectory("Abc.txt", true));
        }
    }
}