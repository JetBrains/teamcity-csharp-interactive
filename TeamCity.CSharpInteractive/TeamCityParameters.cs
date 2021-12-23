// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;
using Contracts;

internal class TeamCityParameters : ITeamCityParameters
{
    private static readonly IReadOnlyDictionary<TeamCityParameterType, string> PropFiles = new Dictionary<TeamCityParameterType, string>
    {
        { TeamCityParameterType.Configuration, "teamcity.configuration.properties.file" },
        { TeamCityParameterType.Build, "teamcity.build.properties.file" },
        { TeamCityParameterType.Runner, "teamcity.build.properties.file" }
    };

    private readonly IDictionary<TeamCityParameterType, IReadOnlyDictionary<string, string>> _properties = new Dictionary<TeamCityParameterType, IReadOnlyDictionary<string, string>>();
    private readonly ITeamCitySettings _teamCitySettings;
    private readonly IHost _host;
    private readonly IFileSystem _fileSystem;
    private readonly IJavaPropertiesParser _parser;

    public TeamCityParameters(
        ITeamCitySettings teamCitySettings,
        IHost host,
        IFileSystem fileSystem,
        IJavaPropertiesParser parser)
    {
        _teamCitySettings = teamCitySettings;
        _host = host;
        _fileSystem = fileSystem;
        _parser = parser;
    }

    public bool TryGetParameter(TeamCityParameterType type, string name, out string value)
    {
        if (_teamCitySettings.IsUnderTeamCity)
        {
            if (!_properties.TryGetValue(type, out var props))
            {
                if (
                    PropFiles.TryGetValue(type, out var propertyName)
                    && _host.Props.TryGetValue(propertyName, out var fileName)
                    && _fileSystem.IsFileExist(fileName))
                {
                    props = _parser.Parse(_fileSystem.ReadAllLines(fileName));
                    _properties.Add(type, props);
                }
            }

            if (props!.TryGetValue(name, out var curValue))
            {
                value = curValue;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }
}