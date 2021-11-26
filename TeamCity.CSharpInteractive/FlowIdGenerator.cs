// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class FlowIdGenerator: IFlowIdGenerator
    {
        private readonly object _lockObject = new();
        private string _nextFlowId; 

        public FlowIdGenerator(ITeamCitySettings teamCitySettings) =>
            _nextFlowId = teamCitySettings.FlowId;

        public string NewFlowId() => GenerateFlowId();

        private string GenerateFlowId()
        {
            lock (_lockObject)
            {
                if (string.IsNullOrWhiteSpace(_nextFlowId))
                {
                    return "cs_" + Guid.NewGuid().ToString()[..8];
                }

                var currentFlowId = _nextFlowId;
                _nextFlowId = string.Empty;
                return currentFlowId;
            }
        }
    }
}