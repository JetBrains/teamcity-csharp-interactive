// ReSharper disable StringLiteralTypo
// ReSharper disable RedundantUsingDirective
namespace TeamCity.CSharpInteractive.Tests.Integration;

using NuGet;
using NuGet.Versioning;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class NuGetTests
{
    [Fact]
    public void ShouldSupportRestore()
    {
        // Given
        var tempPath = CreateTempDirectory();

        // When
        var nuget = Composer.ResolveINuGet();
        var result = nuget.Restore(new RestoreSettings("IoC.Container").WithVersionRange(VersionRange.Parse( "1.3.6")).WithTargetFrameworkMoniker("net5.0").WithPackagesPath(tempPath)).ToList();
            
        // Then
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("IoC.Container");
    }
        
    [Fact]
    public void ShouldSupportRestoreForDefaults()
    {
        // Given
            
        // When
        var nuget = Composer.ResolveINuGet();
        var result = nuget.Restore(new RestoreSettings("IoC.Container")).ToList();
            
        // Then
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ShouldSupportRestoreWhenScript()
    {
        // Given
        var tempPath = CreateTempDirectory();

        // When
        var result = TestTool.Run(
            $"using NuGet;"
            + $"GetService<INuGet>().Restore(\"IoC.Container\", \"1.3.6\", \"net5.0\", @\"{tempPath}\");");
            
        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
    }
        
    private static string CreateTempDirectory() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()[..4]);
}