namespace TeamCity.CSharpInteractive
{
    internal interface ITeamCitySettings
    {
        bool IsUnderTeamCity { get; }

        string Version { get; }
        
        string FlowId { get; }
    }
}