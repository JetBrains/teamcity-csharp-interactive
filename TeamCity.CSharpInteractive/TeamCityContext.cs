// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class TeamCityContext:
    ITeamCityContext,
    DotNet.ISettings
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

    public string DotNetLoggerDirectory => _environment.GetPath(SpecialFolder.Bin);

    public string TeamCityMessagesPath => _teamCitySettings.ServiceMessagesPath;
}