// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Tests;

public class MSBuildArgumentsToolTests
{
    [Theory]
    [InlineData("Abc%2CXyz", "Abc,Xyz")]
    [InlineData("Abc%2C", "Abc,")]
    [InlineData("%2CXyz", ",Xyz")]
    [InlineData("%2C", ",")]
    [InlineData("%%2C", "%,")]
    [InlineData("", "")]
    [InlineData("  ", "  ")]
    [InlineData("Abc%2C%2DXyz", "Abc,-Xyz")]
    [InlineData("%2", "%2")]
    [InlineData("%", "%")]
    [InlineData("%2G", "%2G")]
    [InlineData("%GC", "%GC")]
    [InlineData("% 2C", "% 2C")]
    public void ShouldUnescape(string escaped, string expectedResult)
    {
        // Given
        var tool = CreateInstance();

        // When
        var actualResult = tool.Unescape(escaped);

        // Then
        actualResult.ShouldBe(expectedResult);
    }

    private static MSBuildArgumentsTool CreateInstance() => new();
}