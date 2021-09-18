// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
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
            if (!_isFirst)
            {
                return Guid.NewGuid().ToString().Replace("-", string.Empty);
            }

            _isFirst = false;
            var flowId = _teamCitySettings.FlowId;
            return string.IsNullOrWhiteSpace(flowId) ? Guid.NewGuid().ToString().Replace("-", string.Empty) : flowId;
        }
    }
}