namespace TeamCity.CSharpInteractive.Tests;

public class FileExplorerTests
{
    private readonly Mock<IHostEnvironment> _hostEnvironment;
    private readonly Mock<IFileSystem> _fileSystem;
    
    public FileExplorerTests()
    {
        _hostEnvironment = new Mock<IHostEnvironment>();
        _fileSystem = new Mock<IFileSystem>();
    }

    [Fact]
    public void ShouldFindFilesFromAdditionalPaths()
    {
        // Given
        var explorer = CreateInstance();
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("Abc")).Returns(default(string));
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("DOTNET_HOME")).Returns("DotNet");
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("PATH")).Returns(default(string));
        _fileSystem.Setup(i => i.IsDirectoryExist("DotNet")).Returns(true);
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("DotNet", "*", System.IO.SearchOption.TopDirectoryOnly)).Returns(new []{ "ab", "C", "dd" });
        _fileSystem.Setup(i => i.IsFileExist("ab")).Returns(false);
        _fileSystem.Setup(i => i.IsFileExist("C")).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist("dd")).Returns(true);

        // When
        var actual = explorer.FindFiles("*", "Abc", "DOTNET_HOME").ToArray();

        // Then
        actual.ShouldBe(new []{ "C", "dd" });
    }
    
    [Fact]
    public void ShouldFindFiles()
    {
        // Given
        var explorer = CreateInstance();
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("Abc")).Returns(default(string));
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("DOTNET_HOME")).Returns("DotNet");
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("PATH")).Returns("Bin1; bin2");
        _fileSystem.Setup(i => i.IsDirectoryExist("DotNet")).Returns(true);
        _fileSystem.Setup(i => i.IsDirectoryExist("Bin1")).Returns(false);
        _fileSystem.Setup(i => i.IsDirectoryExist("bin2")).Returns(true);
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("DotNet", "*", System.IO.SearchOption.TopDirectoryOnly)).Returns(new []{ "ab", "C", "dd" });
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("bin2", "*", System.IO.SearchOption.TopDirectoryOnly)).Returns(new []{ "Zz", "zz" });
        _fileSystem.Setup(i => i.IsFileExist("ab")).Returns(false);
        _fileSystem.Setup(i => i.IsFileExist("C")).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist("dd")).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist("Zz")).Returns(false);
        _fileSystem.Setup(i => i.IsFileExist("zz")).Returns(true);

        // When
        var actual = explorer.FindFiles("*", "Abc", "DOTNET_HOME").ToArray();

        // Then
        actual.ShouldBe(new []{ "C", "dd", "zz" });
    }
    
    [Fact]
    public void ShouldSkipDuplicates()
    {
        // Given
        var explorer = CreateInstance();
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("Abc")).Returns(default(string));
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("DOTNET_HOME")).Returns("DotNet");
        _hostEnvironment.Setup(i => i.GetEnvironmentVariable("PATH")).Returns(default(string));
        _fileSystem.Setup(i => i.IsDirectoryExist("DotNet")).Returns(true);
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("DotNet", "*", System.IO.SearchOption.TopDirectoryOnly)).Returns(new []{ "ab", "C", "dd", "C" });
        _fileSystem.Setup(i => i.IsFileExist("ab")).Returns(false);
        _fileSystem.Setup(i => i.IsFileExist("C")).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist("dd")).Returns(true);

        // When
        var actual = explorer.FindFiles("*", "Abc", "DOTNET_HOME").ToArray();

        // Then
        actual.ShouldBe(new []{ "C", "dd" });
    }

    private FileExplorer CreateInstance() =>
        new(_hostEnvironment.Object, _fileSystem.Object);
}