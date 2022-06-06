namespace TeamCity.CSharpInteractive.Tests;

public class FilePathResolverTests
{
    private readonly Mock<ILog<FilePathResolver>> _log;
    private readonly Mock<IEnvironment> _environment;
    private readonly Mock<IFileSystem> _fileSystem;
    private readonly List<Text> _errors = new();

    public FilePathResolverTests()
    {
        _log = new Mock<ILog<FilePathResolver>>();
        _log.Setup(i => i.Error(It.IsAny<ErrorId>(), It.IsAny<Text[]>())).Callback<ErrorId, Text[]>((_, text) => _errors.AddRange(text));

        _environment = new Mock<IEnvironment>();
        _fileSystem = new Mock<IFileSystem>();
    }

    [Theory]
    [InlineData(null, "", false)]
    [InlineData("", "", false)]
    [InlineData(" ", "", false)]
    [InlineData("    ", "", false)]
    [InlineData("Rooted", "", false)]
    [InlineData("Existing1", "wd/Existing1", false)]
    [InlineData("Existing2", "sc/Existing2", false)]
    [InlineData("NotExisting", "", true)]
    public void ShouldResolvePathWhenFile(string? path, string expectedPath, bool hasErrors)
    {
        // Given
        var resolver = CreateInstance();
        _fileSystem.Setup(i => i.IsPathRooted(It.IsAny<string>())).Returns(false);
        _fileSystem.Setup(i => i.IsPathRooted("Rooted")).Returns(true);
        _environment.Setup(i => i.GetPath(SpecialFolder.Script)).Returns("sc");
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("wd");
        _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(j => j == "wd/Existing1".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(j => j == "sc/Existing2".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(j => j == "wd/NotExisting".Replace('/', Path.DirectorySeparatorChar)))).Returns(false);

        // When
        var actualResult = resolver.TryResolve(path, out var actualPath);

        // Then
        actualResult.ShouldBe(expectedPath != string.Empty);
        actualPath.ShouldBe(expectedPath.Replace('/', Path.DirectorySeparatorChar));
        _errors.Any().ShouldBe(hasErrors);
    }
    
    [Fact]
    public void ShouldResolvePathWhenSingleScriptFileInDirectory()
    {
        // Given
        var resolver = CreateInstance();
        _fileSystem.Setup(i => i.IsPathRooted("Existing")).Returns(false);
        _environment.Setup(i => i.GetPath(SpecialFolder.Script)).Returns("sc");
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("wd");
        _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(j => j == "sc/Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(false);
        _fileSystem.Setup(i => i.IsDirectoryExist(It.Is<string>(j => j == "sc/Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("sc/Existing".Replace('/', Path.DirectorySeparatorChar), "*.csx", SearchOption.TopDirectoryOnly))
            .Returns(new[]
            {
                "sc/Existing/MyScript.csx".Replace('/', Path.DirectorySeparatorChar)
            });

        // When
        var actualResult = resolver.TryResolve("Existing", out var actualPath);

        // Then
        actualResult.ShouldBeTrue();
        actualPath.ShouldBe("sc/Existing/MyScript.csx".Replace('/', Path.DirectorySeparatorChar));
        _errors.Any().ShouldBeFalse();
    }
    
    [Fact]
    public void ShouldResolvePathWhenSingleScriptFileInDirectoryWhenRooted()
    {
        // Given
        var resolver = CreateInstance();
        _fileSystem.Setup(i => i.IsPathRooted("Existing")).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(j => j == "Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(false);
        _fileSystem.Setup(i => i.IsDirectoryExist(It.Is<string>(j => j == "Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("Existing".Replace('/', Path.DirectorySeparatorChar), "*.csx", SearchOption.TopDirectoryOnly))
            .Returns(new[]
            {
                "Existing/MyScript.csx".Replace('/', Path.DirectorySeparatorChar)
            });

        // When
        var actualResult = resolver.TryResolve("Existing", out var actualPath);

        // Then
        actualResult.ShouldBeTrue();
        actualPath.ShouldBe("Existing/MyScript.csx".Replace('/', Path.DirectorySeparatorChar));
        _errors.Any().ShouldBeFalse();
    }
    
    [Fact]
    public void ShouldRaiseErrorWhenMultipleScriptFilesInDirectory()
    {
        // Given
        var resolver = CreateInstance();
        _fileSystem.Setup(i => i.IsPathRooted("Existing")).Returns(false);
        _environment.Setup(i => i.GetPath(SpecialFolder.Script)).Returns("sc");
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("wd");
        _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(j => j == "sc/Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(false);
        _fileSystem.Setup(i => i.IsDirectoryExist(It.Is<string>(j => j == "sc/Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("sc/Existing".Replace('/', Path.DirectorySeparatorChar), "*.csx", SearchOption.TopDirectoryOnly))
            .Returns(new[]
            {
                "sc/Existing/MyScript1.csx".Replace('/', Path.DirectorySeparatorChar),
                "sc/Existing/MyScript2.csx".Replace('/', Path.DirectorySeparatorChar)
            });

        // When
        var actualResult = resolver.TryResolve("Existing", out var actualPath);

        // Then
        actualResult.ShouldBeFalse();
        _errors.Any().ShouldBeTrue();
    }
    
    [Fact]
    public void ShouldRaiseErrorWhenNoScriptFilesInDirectory()
    {
        // Given
        var resolver = CreateInstance();
        _fileSystem.Setup(i => i.IsPathRooted("Existing")).Returns(false);
        _environment.Setup(i => i.GetPath(SpecialFolder.Script)).Returns("sc");
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("wd");
        _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(j => j == "sc/Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(false);
        _fileSystem.Setup(i => i.IsDirectoryExist(It.Is<string>(j => j == "sc/Existing".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("sc/Existing".Replace('/', Path.DirectorySeparatorChar), "*.csx", SearchOption.TopDirectoryOnly))
            .Returns(Enumerable.Empty<string>());

        // When
        var actualResult = resolver.TryResolve("Existing", out var actualPath);

        // Then
        actualResult.ShouldBeFalse();
        _errors.Any().ShouldBeTrue();
    }

    private FilePathResolver CreateInstance() =>
        new(_log.Object, _environment.Object, _fileSystem.Object);
}