namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class FileCodeSourceTests
    {
        private readonly Mock<IFileTextReader> _reader;
        private readonly Mock<ILog<FileCodeSource>> _log;
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<IWorkingDirectoryContext> _workingDirectoryContext;
        private readonly Mock<IDisposable> _workingDirectoryToken;

        public FileCodeSourceTests()
        {
            _reader = new Mock<IFileTextReader>();
            _log = new Mock<ILog<FileCodeSource>>();
            _environment = new Mock<IEnvironment>();
            _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("wd");
            _workingDirectoryToken = new Mock<IDisposable>();
            _workingDirectoryContext = new Mock<IWorkingDirectoryContext>();
            _workingDirectoryContext.Setup(i => i.OverrideWorkingDirectory(It.IsAny<string>())).Returns(_workingDirectoryToken.Object);
        }

        [Fact]
        public void ShouldProvideFileContent()
        {
            // Given
            _reader.Setup(i => i.ReadLines( Path.Combine("wd", "zx", "Abc"))).Returns(new [] {"content"});
            var source = CreateInstance(Path.Combine("zx", "Abc"));

            // When
            var expectedResult = source.ToArray();

            // Then
            expectedResult.ShouldBe(new []{"content"});
            _workingDirectoryContext.Verify(i => i.OverrideWorkingDirectory(Path.Combine("wd", "zx")));
            _workingDirectoryToken.Verify(i => i.Dispose());
        }
        
        [Fact]
        public void ShouldLogErrorWhenReadingFailed()
        {
            // Given
            var error = new Exception("test");
            _reader.Setup(i => i.ReadLines( Path.Combine("wd", "Abc"))).Throws(error);
            var source = CreateInstance("Abc");

            // When
            var expectedResult = source.ToArray();

            // Then
            expectedResult.ShouldBeEmpty();
            _log.Verify(i => i.Error(ErrorId.File,It.IsAny<Text[]>()));
        }

        private FileCodeSource CreateInstance(string fileName) => 
            new FileCodeSource(_log.Object, _reader.Object, _environment.Object, _workingDirectoryContext.Object) { FileName = fileName};
    }
}