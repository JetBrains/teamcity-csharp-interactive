// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class NugetEnvironment : INugetEnvironment
    {
        private readonly IEnvironment _environment;
        private readonly IDotnetEnvironment _dotnetEnvironment;
        private readonly Lazy<string> _nuGetStoreFolder;
        
        public NugetEnvironment(
            IEnvironment environment,
            IDotnetEnvironment dotnetEnvironment)
        {
            _environment = environment;
            _dotnetEnvironment = dotnetEnvironment;
            _nuGetStoreFolder = new Lazy<string>(GetPathToNuGetStoreFolder);
        }
        
        public string StoreFolder => _nuGetStoreFolder.Value;

        public IEnumerable<string> Sources => new[] {@"https://api.nuget.org/v3/index.json"};

        public IEnumerable<string> FallbackFolders => new[]
        {
            @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\NuGetPackages",
            @"C:\Program Files (x86)\Microsoft\Xamarin\NuGet\",
            @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder"
        };

        private string GetPathToNuGetStoreFolder() =>
            Path.Combine(
                _dotnetEnvironment.Path,
                "store",
                _environment.ProcessArchitecture,
                _dotnetEnvironment.TargetFrameworkMoniker);
    }
}