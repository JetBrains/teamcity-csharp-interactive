namespace TeamCity.CSharpInteractive.Tests;

using Microsoft.CodeAnalysis.Scripting;
using Xunit;

public class ImportsOptionsFactoryTests
{
    [Fact]
    public void ShouldRegisterStaticUsingHost()
    {
        // Given
        var factory = CreateInstance();

        // When
        var options = factory.Create(ScriptOptions.Default);

        // Then
        options.Imports.ShouldBe(new []{"Host"});
    }

    private ImportsOptionsFactory CreateInstance() =>
        new();
}