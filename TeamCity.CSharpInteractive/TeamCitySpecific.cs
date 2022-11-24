// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive;

internal class CISpecific<T> : ICISpecific<T>
{
    private readonly ICISettings _settings;
    private readonly Func<T> _defaultFactory;
    private readonly Func<T> _teamcityFactory;
    private readonly Func<T> _ansiFactory;

    public CISpecific(
        ICISettings settings,
        [Tag("Default")] Func<T> defaultFactory,
        [Tag("TeamCity")] Func<T> teamcityFactory,
        [Tag("Ansi")] Func<T> ansiFactory)
    {
        _settings = settings;
        _defaultFactory = defaultFactory;
        _teamcityFactory = teamcityFactory;
        _ansiFactory = ansiFactory;
    }

    public T Instance => _settings.CIType switch
    {
        CIType.TeamCity => _teamcityFactory(),
        CIType.GitLab => _ansiFactory(),
        _ => _defaultFactory()
    };
}