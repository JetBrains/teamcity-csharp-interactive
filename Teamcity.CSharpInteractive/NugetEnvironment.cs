// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    internal class NugetEnvironment : INugetEnvironment, ITraceSource, IDisposable
    {
        private readonly IEnvironment _environment;
        private readonly IUniqueNameGenerator _uniqueNameGenerator;
        private readonly ICleaner _cleaner;
        private readonly IDotnetEnvironment _dotnetEnvironment;
        private string? _packagePath;
        private IDisposable _packagePathToken = Disposable.Empty;

        public NugetEnvironment(
            IEnvironment environment,
            IUniqueNameGenerator uniqueNameGenerator,
            ICleaner cleaner,
            IDotnetEnvironment dotnetEnvironment)
        {
            _environment = environment;
            _uniqueNameGenerator = uniqueNameGenerator;
            _cleaner = cleaner;
            _dotnetEnvironment = dotnetEnvironment;
        }

        public IEnumerable<string> Sources => new[]
        {
            @"https://api.nuget.org/v3/index.json"
        };

        public IEnumerable<string> FallbackFolders => 
            new[] {Path.Combine(_dotnetEnvironment.Path, "sdk", "NuGetFallbackFolder")}
            .Concat(FallbackFoldersFromEnv)
            .Distinct();

        public string PackagesPath
        {
            get
            {
                var path = _environment.GetEnvironmentVariable("NUGET_PACKAGES")?.Trim();
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }

                if (_packagePath != null)
                {
                    return _packagePath;
                }
                
                _packagePath = Path.Combine(_environment.GetPath(SpecialFolder.Temp), _uniqueNameGenerator.Generate());
                _packagePathToken = _cleaner.Track(_packagePath);
                return _packagePath;
            }
        }

        public void Dispose() => _packagePathToken.Dispose();

        [ExcludeFromCodeCoverage]
        public IEnumerable<Text> GetTrace()
        {
            yield return new Text($"NugetSources: {string.Join(";", Sources)}");
            yield return new Text($"NugetFallbackFolders: {string.Join(";", FallbackFolders)}");
        }

        private IEnumerable<string> FallbackFoldersFromEnv => _environment
            .GetEnvironmentVariable("NUGET_FALLBACK_PACKAGES")
            ?.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(i => i.Trim()) ?? Enumerable.Empty<string>();
    }
}