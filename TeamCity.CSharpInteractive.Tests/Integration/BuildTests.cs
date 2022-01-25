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
            "using HostApi;",
            "using System.Linq;",
            "var buildRunner = GetService<IBuildRunner>();",
            "var result = buildRunner.Run(new DotNetCustom(\"new\", \"mstest\"));",
            "WriteLine(\"Custom=\" + result.ExitCode);",
            "result = buildRunner.Run(new DotNetRestore());",
            "WriteLine(\"Restore=\" + result.Tests.Count());",
            "result = buildRunner.Run(new DotNetBuild());",
            "WriteLine(\"Build=\" + result.Tests.Count());",
            "result = buildRunner.Run(new DotNetTest());",
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