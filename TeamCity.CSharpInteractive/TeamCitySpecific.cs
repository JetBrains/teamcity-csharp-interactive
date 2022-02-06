// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class TeamCitySpecific<T> : ITeamCitySpecific<T>
{
    private readonly ITeamCitySettings _settings;
    private readonly Func<T> _defaultFactory;
    private readonly Func<T> _teamcityFactory;

    public TeamCitySpecific(
        ITeamCitySettings settings,
        [Tag("Default")] Func<T> defaultFactory,
        [Tag("TeamCity")] Func<T> teamcityFactory)
    {
        _settings = settings;
        _defaultFactory = defaultFactory;
        _teamcityFactory = teamcityFactory;
    }

    public T Instance => _settings.IsUnderTeamCity ? _teamcityFactory() : _defaultFactory();
}