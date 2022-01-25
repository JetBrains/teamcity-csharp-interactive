// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;
using NuGet.Versioning;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class NuGetRestoreAdvanced: ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        // $visible=true
        // $tag=11 NuGet API
        // $priority=01
        // $description=Restore a NuGet package by a version range for the specified .NET and path
        // {
        // Adds the namespace "Script.NuGet" to use INuGet
        // ## using NuGet;

        var packagesPath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString()[..4]);

        var settings = new HostApi.NuGetRestore("IoC.Container")
            .WithVersionRange(VersionRange.Parse("[1.3, 1.3.8)"))
            .WithTargetFrameworkMoniker("net5.0")
            .WithPackagesPath(packagesPath);

        IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(settings);
        // }
            
        packages.ShouldNotBeEmpty();
    }
}