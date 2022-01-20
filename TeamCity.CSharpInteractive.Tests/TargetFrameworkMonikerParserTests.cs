namespace TeamCity.CSharpInteractive.Tests;

public class TargetFrameworkMonikerParserTests
{
    [Theory]
    [InlineData("net00", ".NETFramework,Version=v0.0")]
    [InlineData("net11", ".NETFramework,Version=v1.1")]
    [InlineData("net20", ".NETFramework,Version=v2.0")]
    [InlineData("net35", ".NETFramework,Version=v3.5")]
    [InlineData("net40", ".NETFramework,Version=v4.0")]
    [InlineData("net47", ".NETFramework,Version=v4.7")]
    [InlineData("net472", ".NETFramework,Version=v4.7.2")]
    [InlineData("net5.0", ".NETCoreApp,Version=v5.0")]
    [InlineData("net5.1", ".NETCoreApp,Version=v5.1")]
    [InlineData("net6.0", ".NETCoreApp,Version=v6.0")]
    [InlineData("netcoreapp1.0", ".NETCoreApp,Version=v1.0")]
    [InlineData("netcoreapp2.2", ".NETCoreApp,Version=v2.2")]
    [InlineData("netcoreapp3.1", ".NETCoreApp,Version=v3.1")]
    [InlineData("netstandard1.0", ".NETStandard,Version=v1.0")]
    [InlineData("netstandard2.1", ".NETStandard,Version=v2.1")]
    [InlineData("uap10.0.10240", "UAP,Version=v10.0.10240")]
    [InlineData("Abc", "Abc")]
    [InlineData("  Abc ", "Abc")]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData(".NETCoreApp,Version=v5.0", ".NETCoreApp,Version=v5.0")]
    [InlineData(".NETCoreApp,Version=v3.1", ".NETCoreApp,Version=v3.1")]
    public void Should(string tfm, string expectedTargetFrameworkMoniker)
    {
        // Given
        var parser = CreateInstance();

        // When
        var actualTargetFrameworkMoniker = parser.Parse(tfm);

        // Then
        actualTargetFrameworkMoniker.ShouldBe(expectedTargetFrameworkMoniker);
    }

    private static TargetFrameworkMonikerParser CreateInstance() =>
        new();
}