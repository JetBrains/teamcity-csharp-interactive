// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;

    internal class TeamCityContext:
        ITeamCityContext,
        Dotnet.ISettings
    {
        private readonly IEnvironment _environment;
        private readonly IDotnetEnvironment _dotnetEnvironment;
        private readonly ITeamCitySettings _teamCitySettings;
        [ThreadStatic] private static bool _teamCityIntegration;

        public TeamCityContext(
            IEnvironment environment,
            IDotnetEnvironment dotnetEnvironment,
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

        public string DotnetExecutablePath => _dotnetEnvironment.Path;

        public string DotnetLoggerDirectory => _environment.GetPath(SpecialFolder.Bin);

        public string TeamCityMessagesPath => _teamCitySettings.ServiceMessagesPath;
    }
}