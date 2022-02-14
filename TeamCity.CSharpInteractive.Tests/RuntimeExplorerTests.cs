namespace TeamCity.CSharpInteractive.Tests;

using Xunit;

public class RuntimeExplorerTests
{
    private readonly Mock<IFileSystem> _fileSystem = new();
    
    [Fact]
    public void ShouldFindRuntime()
    {
        // Given
        var explorer = CreateInstance();
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("Runtime", "Abc.dll", SearchOption.TopDirectoryOnly)).Returns(new[] {"Xyz.dll", "Hjk"});

        // When
        explorer.TryFindRuntimeAssembly(Path.Combine("Bin", "Abc.dll"), out var runtimePath).ShouldBeTrue();

        // Then
        runtimePath.ShouldBe("Xyz.dll");
    }
    
    [Fact]
    public void ShouldReturnFalseWhenCannotFindRuntime()
    {
        // Given
        var explorer = CreateInstance();
        _fileSystem.Setup(i => i.EnumerateFileSystemEntries("Runtime", "Abc.dll", SearchOption.TopDirectoryOnly)).Returns(Array.Empty<string>());

        // When
        explorer.TryFindRuntimeAssembly(Path.Combine("Bin", "Abc.dll"), out var runtimePath).ShouldBeFalse();

        // Then
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldReturnFalseWhenRuntimePathIsEmpty(string runtime)
    {
        // Given
        var explorer = CreateInstance(runtime);

        // When
        explorer.TryFindRuntimeAssembly(Path.Combine("Bin", "Abc.dll"), out var runtimePath).ShouldBeFalse();

        // Then
        _fileSystem.Verify(i => i.EnumerateFileSystemEntries(It.IsAny<string>(), It.IsAny<string>(), SearchOption.TopDirectoryOnly), Times.Never);
    }

    private RuntimeExplorer CreateInstance(string runtime = "Runtime") =>
        new(runtime, _fileSystem.Object);
}