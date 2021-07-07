namespace Teamcity.Host
{
    internal interface ITeamCitySettings
    {
        bool IsUnderTeamCity { get; }
        
        string FlowId { get; }
    }
}