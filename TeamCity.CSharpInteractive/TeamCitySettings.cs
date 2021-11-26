// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    internal class TeamCitySettings : ITeamCitySettings
    {
        private readonly IHostEnvironment _hostEnvironment;

        public TeamCitySettings(IHostEnvironment hostEnvironment) => _hostEnvironment = hostEnvironment;

        public string VersionVariableName => "TEAMCITY_VERSION";
        
        public string FlowIdEnvironmentVariableName => "TEAMCITY_PROCESS_FLOW_ID";

        public bool IsUnderTeamCity =>
            !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME"))
            || !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable(VersionVariableName));
        
        public string Version => (_hostEnvironment.GetEnvironmentVariable(VersionVariableName) ?? string.Empty).Trim();
        
        public string FlowId
        {
            get
            {
                var flowId = _hostEnvironment.GetEnvironmentVariable(FlowIdEnvironmentVariableName);
                return string.IsNullOrWhiteSpace(flowId) ? "ROOT" : flowId;
            }
        }
    }
}