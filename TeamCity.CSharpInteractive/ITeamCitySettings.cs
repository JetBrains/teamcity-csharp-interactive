// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    internal interface ITeamCitySettings
    {
        bool IsUnderTeamCity { get; }

        string Version { get; }
        
        string FlowId { get; }

        string ServiceMessagesPath { get; }
    }
}