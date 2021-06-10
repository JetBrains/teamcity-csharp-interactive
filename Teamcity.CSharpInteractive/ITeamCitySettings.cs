namespace Teamcity.CSharpInteractive
{
    internal interface ITeamCitySettings
    {
        bool IsUnderTeamCity { get; }
        
        string FlowId { get; }
    }
}