// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections;
using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

internal class TeamCityProperties : IProperties
{
    private readonly IProperties _props;
    private readonly ITeamCityWriter _teamCityWriter;

    public TeamCityProperties(
        [Tag("Default")] IProperties properties,
        ITeamCityWriter teamCityWriter)
    {
        _props = properties;
        _teamCityWriter = teamCityWriter;
    }

    public int Count => _props.Count;

    public string this[string key]
    {
        get => _props[key];
        set
        {
            _props[key] = value;
            _teamCityWriter.WriteBuildParameter($"system.{key}", value);
        }
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _props.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_props).GetEnumerator();

    public bool TryGetValue(string key, out string value) => _props.TryGetValue(key, out value);
}