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
        private readonly Mock<IFilePathResolver> _filePathResolver;
        private readonly Mock<IScriptContext> _workingDirectoryContext;
        private readonly Mock<IDisposable> _workingDirectoryToken;

        public FileCodeSourceTests()
        {
            _reader = new Mock<IFileTextReader>();
            _log = new Mock<ILog<FileCodeSource>>();
            _filePathResolver = new Mock<IFilePathResolver>();
            _workingDirectoryToken = new Mock<IDisposable>();
            _workingDirectoryContext = new Mock<IScriptContext>();
            _workingDirectoryContext.Setup(i => i.OverrideScriptDirectory(It.IsAny<string>())).Returns(_workingDirectoryToken.Object);
        }

        [Fact]
        public void ShouldProvideFileContent()
        {
            // Given
            string fullPath = Path.Combine("wd", "zx", "Abc");
            _filePathResolver.Setup(i => i.TryResolve(Path.Combine("zx", "Abc"), out fullPath)).Returns(true);
            _reader.Setup(i => i.ReadLines( Path.Combine("wd", "zx", "Abc"))).Returns(new [] {"content"});
            var source = CreateInstance(Path.Combine("zx", "Abc"));

            // When
            var expectedResult = source.ToArray();

            // Then
            expectedResult.ShouldBe(new []{"content"});
            _workingDirectoryContext.Verify(i => i.OverrideScriptDirectory(Path.Combine("wd", "zx")));
            _workingDirectoryToken.Verify(i => i.Dispose());
        }
        
        [Fact]
        public void ShouldLogErrorWhenReadingFailed()
        {
            // Given
            var error = new Exception("test");
            string fullPath = Path.Combine("wd", "Abc");
            _filePathResolver.Setup(i => i.TryResolve("Abc", out fullPath)).Returns(true);
            _reader.Setup(i => i.ReadLines( Path.Combine("wd", "Abc"))).Throws(error);
            var source = CreateInstance("Abc");

            // When
            var expectedResult = source.ToArray();

            // Then
            expectedResult.ShouldBeEmpty();
            _log.Verify(i => i.Error(ErrorId.File,It.IsAny<Text[]>()));
        }

        private FileCodeSource CreateInstance(string fileName) => 
            new FileCodeSource(_log.Object, _reader.Object, _filePathResolver.Object, _workingDirectoryContext.Object) { FileName = fileName};
    }
}