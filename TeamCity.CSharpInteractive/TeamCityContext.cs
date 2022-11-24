// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi.DotNet;

internal class TeamCityContext :
    ITeamCityContext,
    IDotNetSettings
{
    private readonly IEnvironment _environment;
    private readonly IDotNetEnvironment _dotnetEnvironment;
    private readonly ICISettings _ciSettings;
    [ThreadStatic] private static bool _teamCityIntegration;

    public TeamCityContext(
        IEnvironment environment,
        IDotNetEnvironment dotnetEnvironment,
        ICISettings ciSettings)
    {
        _environment = environment;
        _dotnetEnvironment = dotnetEnvironment;
        _ciSettings = ciSettings;
    }

    public bool TeamCityIntegration
    {
        set => _teamCityIntegration = value;
    }

    public bool LoggersAreRequired => _teamCityIntegration;

    public string DotNetExecutablePath => _dotnetEnvironment.Path;

    public string DotNetMSBuildLoggerDirectory => Path.Combine(_environment.GetPath(SpecialFolder.Bin), "msbuild");

    public string DotNetVSTestLoggerDirectory => Path.Combine(_environment.GetPath(SpecialFolder.Bin), "vstest");

    public string TeamCityMessagesPath => _ciSettings.ServiceMessagesPath;
}