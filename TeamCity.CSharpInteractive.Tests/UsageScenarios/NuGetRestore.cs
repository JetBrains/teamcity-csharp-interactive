// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System.Collections.Generic;
    using NuGet;
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
            // Adds the namespace "NuGet" to use INuGet
            // ## using NuGet;

            IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore("IoC.Container", "*");
            // }
            
            packages.ShouldNotBeEmpty();
        }
    }
}