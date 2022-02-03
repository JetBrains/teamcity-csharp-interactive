// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using HostApi;
using NuGet.Versioning;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class NuGetRestore : ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        // $visible=true
        // $tag=11 NuGet API
        // $priority=00
        // $description=Restore NuGet a package of newest version
        // {
        // Adds the namespace "HostApi" to use INuGet
        // ## using HostApi;

        IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(new HostApi.NuGetRestoreSettings("IoC.Container").WithVersionRange(VersionRange.All));
        // }

        packages.ShouldNotBeEmpty();
    }
}