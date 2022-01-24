namespace TeamCity.CSharpInteractive.Tests.Integration;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class BuildTests
{
    [Fact]
    public void ShouldRunCustomRestoreBuildTest()
    {
        // Given

        // When
        var result = TestTool.Run(
            "using Script.DotNet;",
            "using System.Linq;",
            "var build = GetService<IBuild>();",
            "var result = build.Run(new Custom(\"new\", \"mstest\"));",
            "WriteLine(\"Custom=\" + result.ExitCode);",
            "result = build.Run(new Restore());",
            "WriteLine(\"Restore=\" + result.Tests.Count());",
            "result = build.Run(new Build());",
            "WriteLine(\"Build=\" + result.Tests.Count());",
            "result = build.Run(new Test());",
            "WriteLine(\"Tests=\" + result.Tests.Count());"
        );

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Contains("Custom=0").ShouldBeTrue();
        result.StdOut.Contains("Restore=0").ShouldBeTrue();
        result.StdOut.Contains("Build=0").ShouldBeTrue();
        result.StdOut.Contains("Tests=1").ShouldBeTrue();
    }
}