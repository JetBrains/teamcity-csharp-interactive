namespace TeamCity.CSharpInteractive
{
    internal interface ITeamCitySettings
    {
        bool IsUnderTeamCity { get; }
        
        string FlowId { get; }
    }
}