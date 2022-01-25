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

        // When
        var result = TestTool.Run("return 33;");
            
        // Then
        result.ExitCode.ShouldBe(33, result.ToString());
    }

    [Fact]
    public void ShouldGetExitCodeFromEnvironmentExit()
    {
        // Given

        // When
        var result = TestTool.Run("Environment.Exit(33);");
            
        // Then
        result.ExitCode.ShouldBe(33, result.ToString());
    }

    [Fact]
    public void ExitCodeShouldBe1WhenError()
    {
        // Given

        // When
        var result = TestTool.Run("throw new Exception();");
            
        // Then
        result.ExitCode.ShouldBe(1, result.ToString());
    }
    
    [Fact]
    public void ExitCodeShouldBe0ByDefault()
    {
        // Given

        // When
        var result = TestTool.Run("var i=10;");
            
        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
    }
}