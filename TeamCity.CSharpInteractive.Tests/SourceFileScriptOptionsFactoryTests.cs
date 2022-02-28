namespace TeamCity.CSharpInteractive.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Xunit;

public class SourceFileScriptOptionsFactoryTests
{
    private readonly Mock<Func<SourceReferenceResolver>> _sourceReferenceResolverFactory = new();

    [Fact]
    public void ShouldSetupSourceFileResolver()
    {
        // Given
        var factory = CreateInstance();
        var resolver = Mock.Of<SourceReferenceResolver>();
        _sourceReferenceResolverFactory.Setup(i => i()).Returns(resolver);

        // When
        var options = factory.Create(ScriptOptions.Default);
        var actualResolver = options.SourceResolver;

        // Then
        actualResolver.ShouldBe(resolver);
    }

    private SourceFileScriptOptionsFactory CreateInstance() =>
        new(_sourceReferenceResolverFactory.Object);
}