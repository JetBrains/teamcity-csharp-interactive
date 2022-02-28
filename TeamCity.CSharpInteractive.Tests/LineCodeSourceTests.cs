namespace TeamCity.CSharpInteractive.Tests;

using Xunit;

public class LineCodeSourceTests
{
    [Fact]
    public void Should()
    {
        // Given
        var source = CreateInstance();

        // When
        source.Line = "Abc";

        // Then
        source.Name.ShouldBe("Abc");
        source.ToArray().ShouldBe(new []{"Abc"});
    }

    private static LineCodeSource CreateInstance() =>
        new();
}