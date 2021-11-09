// ReSharper disable StringLiteralTypo
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    using System;
    using Contracts;
    using Shouldly;
    using Xunit;

    public class NuGetRestoreAdvanced: Scenario
    {
        [SkippableFact]
        public void Run()
        {
            // $visible=true
            // $tag=1 NuGet API
            // $priority=01
            // $description=Restore NuGet the package of version in the range for the specified .NET to a path
            // {
            var packagesPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()[..4]); 
            var packages = GetService<INuGet>().Restore("IoC.Container", "[1.3, 1.3.8)", "net5.0", packagesPath);
            // }
            
            packages.ShouldNotBeEmpty();
        }
    }
}