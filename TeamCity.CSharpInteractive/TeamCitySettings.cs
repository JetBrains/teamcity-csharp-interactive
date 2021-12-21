// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    internal class TeamCitySettings : ITeamCitySettings
    {
        private const string VersionVariableName = "TEAMCITY_VERSION";
        private const string ProjectNameVariableName = "TEAMCITY_PROJECT_NAME";
        private const string FlowIdEnvironmentVariableName = "TEAMCITY_PROCESS_FLOW_ID";
        private const string DefaultFlowId = "ROOT";
        private readonly IHostEnvironment _hostEnvironment;

        public TeamCitySettings(IHostEnvironment hostEnvironment) => _hostEnvironment = hostEnvironment;

        public bool IsUnderTeamCity =>
            !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable(ProjectNameVariableName))
            || !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable(VersionVariableName));
        
        public string Version => (_hostEnvironment.GetEnvironmentVariable(VersionVariableName) ?? string.Empty).Trim();
        
        public string FlowId
        {
            get
            {
                var flowId = _hostEnvironment.GetEnvironmentVariable(FlowIdEnvironmentVariableName);
                return string.IsNullOrWhiteSpace(flowId) ? DefaultFlowId : flowId;
            }
        }
    }
}