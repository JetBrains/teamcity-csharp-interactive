namespace TeamCity.CSharpInteractive.Tests;

using Xunit;

public class EnvironmentTests
{
    [Fact]
    public void ShouldGetSourceNameWhenSeveralSources()
    {
        // Given
        var environment = CreateInstance();
        var source = new Mock<ICodeSource>();
        source.SetupGet(i => i.Name).Returns(Path.Combine("Dir", "Abc.txt"));
        
        var source2 = new Mock<ICodeSource>();
        source2.SetupGet(i => i.Name).Returns(Path.Combine("Dir", "Abc2.txt"));

        // When
        using var scope = environment.CreateScope(source.Object);
        using var scope2 = environment.CreateScope(source2.Object);

        // Then
        environment.TryGetSourceName(out var name).ShouldBeTrue();
        name.ShouldBe("Abc2.txt");
    }
    
    [Fact]
    public void ShouldGetSourceName()
    {
        // Given
        var environment = CreateInstance();
        var source = new Mock<ICodeSource>();
        source.SetupGet(i => i.Name).Returns(Path.Combine("Dir", "Abc.txt"));

        // When
        using var scope = environment.CreateScope(source.Object);

        // Then
        environment.TryGetSourceName(out var name).ShouldBeTrue();
        name.ShouldBe("Abc.txt");
    }
    
    [Fact]
    public void ShouldNotGetSourceNameWhenNoSources()
    {
        // Given
        var environment = CreateInstance();

        // When

        // Then
        environment.TryGetSourceName(out _).ShouldBeFalse();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldNotGetSourceNameWhenNameIsEmpty(string path)
    {
        // Given
        var environment = CreateInstance();
        var source = new Mock<ICodeSource>();
        source.SetupGet(i => i.Name).Returns(path);
        
        // When
        using var scope = environment.CreateScope(source.Object);

        // Then
        environment.TryGetSourceName(out _).ShouldBeFalse();
    }
    
    [Fact]
    public void ShouldProvideScriptFolder()
    {
        // Given
        var environment = CreateInstance();
        var source = new Mock<ICodeSource>();
        source.SetupGet(i => i.Name).Returns(Path.Combine("Dir1", "Abc.txt"));
        
        var source2 = new Mock<ICodeSource>();
        source2.SetupGet(i => i.Name).Returns(Path.Combine("Dir2", "Abc2.txt"));

        // When
        using var scope = environment.CreateScope(source.Object);
        using var scope2 = environment.CreateScope(source2.Object);

        // Then
        environment.GetPath(SpecialFolder.Script).ShouldBe("Dir2");
    }
    
    [Fact]
    public void ShouldProvideScriptFolderWhenHasDir()
    {
        // Given
        var environment = CreateInstance();
        var source = new Mock<ICodeSource>();
        source.SetupGet(i => i.Name).Returns(Path.Combine("Dir1", "Abc.txt"));
        
        // When
        using var scope = environment.CreateScope(source.Object);
        
        // Then
        environment.GetPath(SpecialFolder.Script).ShouldBe("Dir1");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldProvideWorkingDirectoryAsScriptDirectoryWhenSourceIsEmpty(string sourceValue)
    {
        // Given
        var environment = CreateInstance();
        var source = new Mock<ICodeSource>();
        source.SetupGet(i => i.Name).Returns(sourceValue);

        // When
        using var scope = environment.CreateScope(source.Object);
        
        // Then
        environment.GetPath(SpecialFolder.Script).ShouldBe(Directory.GetCurrentDirectory());
    }
    
    [Fact]
    public void ShouldProvideScriptAsFolderWhenHasNoDir()
    {
        // Given
        var environment = CreateInstance();
        var source = new Mock<ICodeSource>();
        source.SetupGet(i => i.Name).Returns("Abc.txt");
        
        // When
        using var scope = environment.CreateScope(source.Object);
        
        // Then
        environment.GetPath(SpecialFolder.Script).ShouldBe("Abc.txt");
    }

    private static Environment CreateInstance() =>
        new();
}