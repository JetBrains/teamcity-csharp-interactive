// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.IO;

    internal class NugetEnvironment : INugetEnvironment
    {
        private readonly IDotnetEnvironment _dotnetEnvironment;

        public NugetEnvironment(
            IDotnetEnvironment dotnetEnvironment) =>
            _dotnetEnvironment = dotnetEnvironment;

        public IEnumerable<string> Sources => new[]
        {
            @"https://api.nuget.org/v3/index.json"
        };

        public IEnumerable<string> FallbackFolders => new[]
        {
            Path.Combine(_dotnetEnvironment.Path, "sdk", "NuGetFallbackFolder")
        };
    }
}