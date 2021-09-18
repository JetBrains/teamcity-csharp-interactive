// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    internal class TeamCitySettings : ITeamCitySettings
    {
        private readonly IHostEnvironment _hostEnvironment;

        public TeamCitySettings(IHostEnvironment hostEnvironment) => _hostEnvironment = hostEnvironment;

        public bool IsUnderTeamCity =>
            !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME"))
            || !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable("TEAMCITY_VERSION"));
        
        public string FlowId => (_hostEnvironment.GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID") ?? string.Empty).Trim();
    }
}