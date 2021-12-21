// ReSharper disable StringLiteralTypo
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using System.Collections.Generic;
    using NuGet;
    using Shouldly;
    using Xunit;

    public class NuGetRestoreAdvanced: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            // $visible=true
            // $tag=11 NuGet API
            // $priority=01
            // $description=Restore a NuGet package by a version range for the specified .NET and path
            // {
            // Adds the namespace "NuGet" to use INuGet
            // ## using NuGet;

            var packagesPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                Guid.NewGuid().ToString()[..4]);

            IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(
                "IoC.Container",
                "[1.3, 1.3.8)",
                "net5.0",
                packagesPath);
            // }
            
            packages.ShouldNotBeEmpty();
        }
    }
}