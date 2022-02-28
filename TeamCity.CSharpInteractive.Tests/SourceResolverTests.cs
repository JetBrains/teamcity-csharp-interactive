namespace TeamCity.CSharpInteractive.Tests;

using System.IO;
using Xunit;

public class SourceResolverTests
{
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IScriptContentReplacer> _scriptContentReplacer = new();
    private readonly Mock<ITextReplacer> _textReplacer = new();
    private readonly MemoryStream _openReadStream = new();

    public SourceResolverTests()
    {
        _environment.Setup(i => i.GetPath(SpecialFolder.Script)).Returns("ScriptDir");
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("WorkingDir");
    }

    [Fact]
    public void ShouldSetupBaseAndSearchDirectories()
    {
        // Given
        var resolver = CreateInstance();

        // When

        // Then
        resolver.BaseDirectory.ShouldBe(Path.GetFullPath("ScriptDir"));
        resolver.SearchPaths.ShouldBe(new []{Path.GetFullPath("ScriptDir"), Path.GetFullPath("WorkingDir")});
    }
    
    [Fact]
    public void ShouldReplaceScript()
    {
        // Given
        MemoryStream replacedStream = new();
        _textReplacer.Setup(i => i.Replace(_openReadStream, _scriptContentReplacer.Object.Replace)).Returns(replacedStream);
        var resolver = CreateInstance();
        
        // When
        var actualStream = resolver.OpenRead("Script.csx");
        
        // Then
        actualStream.ShouldBe(replacedStream);
    }

    private TestSourceResolver CreateInstance() =>
        new(_environment.Object, _scriptContentReplacer.Object, _textReplacer.Object, _openReadStream);
    
    private class TestSourceResolver: SourceResolver
    {
        private readonly Stream _openReadStream;
        public TestSourceResolver(IEnvironment environment, IScriptContentReplacer scriptContentReplacer, ITextReplacer textReplacer, Stream openReadStream)
            : base(environment, scriptContentReplacer, textReplacer) =>
            _openReadStream = openReadStream;

        protected override Stream OpenReadInternal(string resolvedPath) => _openReadStream;
    }
}