namespace TeamCity.CSharpInteractive
{
    internal interface ITeamCityLineFormatter
    {
        string Format(params Text[] line);
    }
}