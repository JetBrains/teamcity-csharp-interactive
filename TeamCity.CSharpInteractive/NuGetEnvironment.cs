// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

internal class NuGetEnvironment : INuGetEnvironment, ITraceSource, IDisposable
{
    private readonly IEnvironment _environment;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IUniqueNameGenerator _uniqueNameGenerator;
    private readonly ICleaner _cleaner;
    private readonly ISettings _settings;
    private string? _packagePath;
    private IDisposable _packagePathToken = Disposable.Empty;

    public NuGetEnvironment(
        IEnvironment environment,
        IHostEnvironment hostEnvironment,
        IUniqueNameGenerator uniqueNameGenerator,
        ICleaner cleaner,
        ISettings settings)
    {
        _environment = environment;
        _hostEnvironment = hostEnvironment;
        _uniqueNameGenerator = uniqueNameGenerator;
        _cleaner = cleaner;
        _settings = settings;
    }

    public IEnumerable<string> Sources => _settings.NuGetSources.Concat(new []{@"https://api.nuget.org/v3/index.json"});

    public IEnumerable<string> FallbackFolders => 
        FallbackFoldersFromEnv
            .Distinct();

    public string PackagesPath
    {
        get
        {
            var path = _hostEnvironment.GetEnvironmentVariable("NUGET_PACKAGES")?.Trim();
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
    public IEnumerable<Text> Trace
    {
        get
        {
            yield return new Text($"PackagesPath: {PackagesPath}");
            yield return Text.NewLine;
            yield return new Text($"NuGetSources: {string.Join(";", Sources)}");
            yield return Text.NewLine;
            yield return new Text($"NuGetFallbackFolders: {string.Join(";", FallbackFolders)}");
            yield return Text.NewLine;
        }
    }

    private IEnumerable<string> FallbackFoldersFromEnv => _hostEnvironment
        .GetEnvironmentVariable("NUGET_FALLBACK_PACKAGES")
        ?.Split(';', StringSplitOptions.RemoveEmptyEntries)
        .Select(i => i.Trim()) ?? Enumerable.Empty<string>();
}