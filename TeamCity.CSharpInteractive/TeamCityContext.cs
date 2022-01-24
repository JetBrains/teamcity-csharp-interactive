// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class TeamCityContext:
    ITeamCityContext,
    Script.DotNet.ISettings
{
    private readonly IEnvironment _environment;
    private readonly IDotNetEnvironment _dotnetEnvironment;
    private readonly ITeamCitySettings _teamCitySettings;
    [ThreadStatic] private static bool _teamCityIntegration;

    public TeamCityContext(
        IEnvironment environment,
        IDotNetEnvironment dotnetEnvironment,
        ITeamCitySettings teamCitySettings)
    {
        _environment = environment;
        _dotnetEnvironment = dotnetEnvironment;
        _teamCitySettings = teamCitySettings;
    }

    public bool TeamCityIntegration
    {
        set => _teamCityIntegration = value;
    }
        
    public bool LoggersAreRequired => _teamCityIntegration;

    public string DotNetExecutablePath => _dotnetEnvironment.Path;

    public string DotNetMSBuildLoggerDirectory => Path.Combine(_environment.GetPath(SpecialFolder.Bin), "msbuild");
    
    public string DotNetVSTestLoggerDirectory => Path.Combine(_environment.GetPath(SpecialFolder.Bin), "vstest");

    public string TeamCityMessagesPath => _teamCitySettings.ServiceMessagesPath;
}