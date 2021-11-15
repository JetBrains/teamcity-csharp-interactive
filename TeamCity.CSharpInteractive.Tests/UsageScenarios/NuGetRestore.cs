// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System.Collections.Generic;
    using Contracts;
    using Shouldly;
    using Xunit;

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
            IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore("IoC.Container", "*");
            // }
            
            packages.ShouldNotBeEmpty();
        }
    }
}