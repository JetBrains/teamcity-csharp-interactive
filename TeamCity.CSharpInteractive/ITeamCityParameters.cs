// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    internal interface ITeamCityParameters
    {
        bool TryGetParameter(TeamCityParameterType type, string name, out string value);
    }
}