namespace TeamCity.CSharpInteractive.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Xunit;

public class SourceFileScriptOptionsFactoryTests
{
    private readonly Mock<IEnvironment> _environment = new();

    [Fact]
    public void ShouldSetupSourceFileResolver()
    {
        // Given
        var factory = CreateInstance();
        _environment.Setup(i => i.GetPath(SpecialFolder.Script)).Returns("ScriptDir");
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("WorkingDir");

        // When
        var options = factory.Create(ScriptOptions.Default);
        var resolver = (SourceFileResolver)options.SourceResolver;

        // Then
        resolver.SearchPaths.ShouldBe(new []{Path.GetFullPath("ScriptDir"), Path.GetFullPath("WorkingDir")});
        resolver.BaseDirectory.ShouldBe(Path.GetFullPath("ScriptDir"));
    }

    private SourceFileScriptOptionsFactory CreateInstance() =>
        new(_environment.Object);
}