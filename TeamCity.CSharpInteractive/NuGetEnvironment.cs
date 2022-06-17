// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class NuGetEnvironment : INuGetEnvironment, ITraceSource
{
    private readonly IEnvironment _environment;
    private readonly ISettings _settings;

    public NuGetEnvironment(
        IEnvironment environment,
        ISettings settings)
    {
        _environment = environment;
        _settings = settings;
    }

    public IEnumerable<string> Sources => 
        _settings.NuGetSources
            .Concat(NuGet.Configuration.SettingsUtility.GetEnabledSources(GetNuGetSettings()).Select(i => i.Source))
            .Distinct();

    public IEnumerable<string> FallbackFolders =>
        NuGet.Configuration.SettingsUtility.GetFallbackPackageFolders(GetNuGetSettings()).Distinct();

    public string PackagesPath =>
        NuGet.Configuration.SettingsUtility.GetGlobalPackagesFolder(GetNuGetSettings());
    
    public IEnumerable<Text> Trace
    {
        get
        {
            yield return new Text($"PackagesPath: {PackagesPath}");
            yield return Text.NewLine;
            yield return new Text($"NuGetSources: {string.Join(';', Sources)}");
            yield return Text.NewLine;
            yield return new Text($"NuGetFallbackFolders: {string.Join(';', FallbackFolders)}");
            yield return Text.NewLine;
        }
    }

    private NuGet.Configuration.ISettings GetNuGetSettings() =>
        NuGet.Configuration.Settings.LoadDefaultSettings(_environment.GetPath(SpecialFolder.Script));
}