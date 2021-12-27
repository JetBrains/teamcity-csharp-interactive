// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using Contracts;

    internal class WellknownValueResolver : IWellknownValueResolver
    {
        private readonly IEnvironment _environment;
        private readonly IDotnetEnvironment _dotnetEnvironment;
        private readonly IDockerEnvironment _dockerEnvironment;
        private readonly ITeamCitySettings _teamCitySettings;

        public WellknownValueResolver(
            IEnvironment environment,
            IDotnetEnvironment dotnetEnvironment,
            IDockerEnvironment dockerEnvironment,
            ITeamCitySettings teamCitySettings)
        {
            _environment = environment;
            _dotnetEnvironment = dotnetEnvironment;
            _dockerEnvironment = dockerEnvironment;
            _teamCitySettings = teamCitySettings;
        }

        public string Resolve(string value) =>
            value
                // Dotnet
                .Replace(WellknownValues.DotnetExecutablePath, _dotnetEnvironment.Path)
                .Replace(WellknownValues.DotnetLoggerDirectory, _environment.GetPath(SpecialFolder.Bin))
                .Replace(WellknownValues.TeamCityVersion, _teamCitySettings.Version)
                .Replace(WellknownValues.TeamCityMessagesPath, _teamCitySettings.ServiceMessagesPath)
                // Docker
                .Replace(WellknownValues.DockerExecutablePath, _dockerEnvironment.Path);
    }
}