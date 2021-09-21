// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using Contracts;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Pure.DI;

    internal class TeamCityProperties: IProperties
    {
        private readonly IProperties _props;
        private readonly ITeamCityBuildStatusWriter _teamCityBuildStatusWriter;

        public TeamCityProperties(
            [Tag("Default")] IProperties properties,
            ITeamCityBuildStatusWriter teamCityBuildStatusWriter)
        {
            _props = properties;
            _teamCityBuildStatusWriter = teamCityBuildStatusWriter;
        }
        
        public int Count => _props.Count;

        public string this[string key]
        {
            get => _props[key];
            set
            {
                _props[key] = value;
                _teamCityBuildStatusWriter.WriteBuildParameter($"system.{key}", value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _props.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_props).GetEnumerator();
        
        public bool TryGetValue(string key, out string value) => _props.TryGetValue(key, out value);
    }
}