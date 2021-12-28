// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class TeamCityService: ITeamCity, ITeamCityContext
    {
        [ThreadStatic] private static bool _teamCityIntegration;

        public bool TeamCityIntegration
        {
            get => _teamCityIntegration;
            set => _teamCityIntegration = value;
        }
    }
}