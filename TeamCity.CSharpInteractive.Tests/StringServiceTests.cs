namespace TeamCity.CSharpInteractive.Tests
{
    using Shouldly;
    using Xunit;

    public class StringServiceTests
    {
        [Theory]
        [InlineData("\"Abc\"", "Abc")]
        [InlineData("Abc", "Abc")]
        [InlineData("\"A bc\"", "A bc")]
        [InlineData(" \"Abc\"  ", "Abc")]
        [InlineData("\"Abc", "\"Abc")]
        [InlineData("Abc\"", "Abc\"")]
        [InlineData(" \" Abc", "\" Abc")]
        [InlineData("  ", "")]
        [InlineData("", "")]
        public void ShouldTrimAndUnquote(string text, string expectedResult)
        {
            // Given
            var stringService = new StringService();
            
            // When
            var actualResult = stringService.TrimAndUnquote(text);

            // Then
            actualResult.ShouldBe(expectedResult);
        }
    }
}