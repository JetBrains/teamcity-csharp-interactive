namespace Teamcity.Host
{
    internal interface ITeamCityLineFormatter
    {
        string Format(params Text[] line);
    }
}