namespace TeamCity.CSharpInteractive
{
    internal interface ITeamCitySpecific<out T>
    {
        T Instance { get; }
    }
}