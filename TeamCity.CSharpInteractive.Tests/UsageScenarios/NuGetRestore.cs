// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using NuGet;
using NuGet.Versioning;
using Script.NuGet;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class NuGetRestore: ScenarioHostService
{
    [SkippableFact]
    public void Run()
    {
        // $visible=true
        // $tag=11 NuGet API
        // $priority=00
        // $description=Restore NuGet a package of newest version
        // {
        // Adds the namespace "Script.NuGet" to use INuGet
        // ## using NuGet;

        IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(new RestoreSettings("IoC.Container").WithVersionRange(VersionRange.All));
        // }
            
        packages.ShouldNotBeEmpty();
    }
}