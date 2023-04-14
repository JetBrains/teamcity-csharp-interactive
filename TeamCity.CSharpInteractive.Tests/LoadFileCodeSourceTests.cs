namespace TeamCity.CSharpInteractive.Tests;

public class LoadFileCodeSourceTests
{
    private readonly Mock<IFilePathResolver> _filePathResolver;
    private readonly Mock<IScriptContext> _workingDirectoryContext;
    private readonly Mock<IDisposable> _workingDirectoryToken;

    public LoadFileCodeSourceTests()
    {
        _filePathResolver = new Mock<IFilePathResolver>();
        _workingDirectoryToken = new Mock<IDisposable>();
        _workingDirectoryContext = new Mock<IScriptContext>();
    }

    [Fact]
    public void ShouldProvideLoadCommand()
    {
        // Given
        var fullPath = Path.Combine("wd", "zx", "Abc");
        _filePathResolver.Setup(i => i.TryResolve(Path.Combine("zx", "Abc"), out fullPath)).Returns(true);
        var source = CreateInstance(Path.Combine("zx", "Abc"));
        _workingDirectoryContext.Setup(i => i.CreateScope(source)).Returns(_workingDirectoryToken.Object);

        // When
        var expectedResult = source.ToArray();

        // Then
        expectedResult.ShouldBe(new[] {$"#load \"{fullPath}\""});
        _workingDirectoryContext.Verify(i => i.CreateScope(source));
        _workingDirectoryToken.Verify(i => i.Dispose());
    }

    private LoadFileCodeSource CreateInstance(string fileName) =>
        new(_filePathResolver.Object, _workingDirectoryContext.Object) {Name = fileName};
}