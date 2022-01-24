// ReSharper disable StringLiteralTypo
// ReSharper disable RedundantUsingDirective
namespace TeamCity.CSharpInteractive.Tests.Integration;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class ExitCodeTests
{
    [Fact]
    public void ShouldGetExitCodeFromReturnValue()
    {
        // Given
        var tempPath = CreateTempDirectory();

        // When
        var result = TestTool.Run("return 33;");
            
        // Then
        result.ExitCode.ShouldBe(33, result.ToString());
    }

    [Fact]
    public void ShouldGetExitCodeFromEnvironmentExit()
    {
        // Given
        var tempPath = CreateTempDirectory();

        // When
        var result = TestTool.Run("Environment.Exit(33);");
            
        // Then
        result.ExitCode.ShouldBe(33, result.ToString());
    }

    [Fact]
    public void ExitCodeShouldBe1WhenError()
    {
        // Given
        var tempPath = CreateTempDirectory();

        // When
        var result = TestTool.Run("throw new Exception();");
            
        // Then
        result.ExitCode.ShouldBe(1, result.ToString());
    }
    
    [Fact]
    public void ExitCodeShouldBe0ByDefault()
    {
        // Given
        var tempPath = CreateTempDirectory();

        // When
        var result = TestTool.Run("var i=10;");
            
        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
    }

    private static string CreateTempDirectory() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()[..4]);
}