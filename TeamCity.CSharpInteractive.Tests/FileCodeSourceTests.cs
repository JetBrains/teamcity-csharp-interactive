namespace TeamCity.CSharpInteractive.Tests;

public class FileCodeSourceTests
{
    private readonly Mock<IFileSystem> _fileSystem;
    private readonly Mock<ILog<FileCodeSource>> _log;
    private readonly Mock<IFilePathResolver> _filePathResolver;
    private readonly Mock<IScriptContext> _workingDirectoryContext;
    private readonly Mock<IDisposable> _workingDirectoryToken;

    public FileCodeSourceTests()
    {
        _fileSystem = new Mock<IFileSystem>();
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
        var fullPath = Path.Combine("wd", "zx", "Abc");
        _filePathResolver.Setup(i => i.TryResolve(Path.Combine("zx", "Abc"), out fullPath)).Returns(true);
        _fileSystem.Setup(i => i.ReadAllLines( Path.Combine("wd", "zx", "Abc"))).Returns(new [] {"content"});
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
        var fullPath = Path.Combine("wd", "Abc");
        _filePathResolver.Setup(i => i.TryResolve("Abc", out fullPath)).Returns(true);
        _fileSystem.Setup(i => i.ReadAllLines(Path.Combine("wd", "Abc"))).Throws(error);
        var source = CreateInstance("Abc");

        // When
        var expectedResult = source.ToArray();

        // Then
        expectedResult.ShouldBeEmpty();
        _log.Verify(i => i.Error(ErrorId.File,It.IsAny<Text[]>()));
    }

    private FileCodeSource CreateInstance(string fileName) => 
        new(_log.Object, _fileSystem.Object, _filePathResolver.Object, _workingDirectoryContext.Object) { FileName = fileName};
}