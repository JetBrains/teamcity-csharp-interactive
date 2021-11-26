namespace TeamCity.CSharpInteractive
{
    internal interface ITeamCitySettings
    {
        bool IsUnderTeamCity { get; }

        string VersionVariableName { get; }

        string FlowIdEnvironmentVariableName { get; }

        string Version { get; }
        
        string FlowId { get; }
    }
}