// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class NuGetRestore: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            // $visible=true
            // $tag=1 NuGet API
            // $priority=00
            // $description=Restore NuGet a package of newest version
            // {
            var packagesPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()[..4]); 
            var packages = GetService<INuGet>().Restore("IoC.Container");
            // }
            
            packages.ShouldNotBeEmpty();
        }
    }
}