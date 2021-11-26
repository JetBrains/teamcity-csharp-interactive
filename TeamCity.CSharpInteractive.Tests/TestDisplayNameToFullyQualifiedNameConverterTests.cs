namespace TeamCity.CSharpInteractive.Tests
{
    using Shouldly;
    using Xunit;

    public class TestDisplayNameToFullyQualifiedNameConverterTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("   ", "   ")]
        [InlineData("MSTest.Tests.Test1", "MSTest.Tests.Test1")]
        [InlineData("MSTest.Tests.Test1(Aa,Bb)", "MSTest.Tests.Test1")]
        [InlineData("MSTest.Tests.Test1 (Aa,Bb)", "MSTest.Tests.Test1")]
        [InlineData("MSTest.Tests.Test1<string, int>(Aa,Bb)", "MSTest.Tests.Test1")]
        [InlineData("MSTest.Tests.Test1<string, int> (Aa,Bb)", "MSTest.Tests.Test1")]
        [InlineData("MSTest.Tests.Test1(Aa,Bb", "MSTest.Tests.Test1(Aa,Bb")]
        [InlineData("MSTest.Tests.Test1(Aa(,Bb)", "MSTest.Tests.Test1")]
        [InlineData("a(Aa,Bb)", "a")]
        [InlineData("a<int>(Aa,Bb)", "a")]
        [InlineData("(Aa,Bb)", "(Aa,Bb)")]
        [InlineData(" (Aa,Bb)", " (Aa,Bb)")]
        public void Should(string displayName, string expectedFullyQualifiedName)
        {
            // Given
            var converter = CreateInstance();

            // When
            var actualFullyQualifiedName = converter.Convert(displayName);

            // Then
            actualFullyQualifiedName.ShouldBe(expectedFullyQualifiedName);
        }

        private static TestDisplayNameToFullyQualifiedNameConverter CreateInstance() =>
            new();
    }
}