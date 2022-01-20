// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using NuGet;
using NuGet.Versioning;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class NuGetRestore: Scenario
{
    [SkippableFact]
    public void Run()
    {
        // $visible=true
        // $tag=11 NuGet API
        // $priority=00
        // $description=Restore NuGet a package of newest version
        // {
        // Adds the namespace "NuGet" to use INuGet
        // ## using NuGet;

        IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(new RestoreSettings("IoC.Container").WithVersionRange(VersionRange.All));
        // }
            
        packages.ShouldNotBeEmpty();
    }
}