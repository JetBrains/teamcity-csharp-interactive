// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class FlowIdGenerator: IFlowIdGenerator
    {
        private readonly ITeamCitySettings _teamCitySettings;
        private bool _isFirst = true;

        public FlowIdGenerator(ITeamCitySettings teamCitySettings) => _teamCitySettings = teamCitySettings;

        public string NewFlowId()
        {
            if (_isFirst)
            {
                _isFirst = false;
                var flowId = _teamCitySettings.FlowId;
                if (!string.IsNullOrWhiteSpace(flowId))
                {
                    return flowId;
                }
            }
            
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}