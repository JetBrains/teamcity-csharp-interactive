namespace Teamcity.CSharpInteractive
{
    internal interface ITeamCityLineFormatter
    {
        string Format(params Text[] line);
    }
}