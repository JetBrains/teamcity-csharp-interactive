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
        options.Imports.Length.ShouldBe(1);
        (options.Imports.Contains("Host") || options.Imports.Contains("Components")).ShouldBeTrue();
    }

    private static ImportsOptionsFactory CreateInstance() =>
        new();
}