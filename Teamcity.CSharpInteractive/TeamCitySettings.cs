// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    internal class TeamCitySettings : ITeamCitySettings
    {
        private readonly IEnvironment _environment;

        public TeamCitySettings(IEnvironment environment) => _environment = environment;

        public bool IsUnderTeamCity =>
            !string.IsNullOrWhiteSpace(_environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME"))
            || !string.IsNullOrWhiteSpace(_environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
        
        public string FlowId => (_environment.GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID") ?? string.Empty).Trim();
    }
}