// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using Cmd;

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

        public string Resolve(WellknownValue value) =>
            value switch
            {
                // Dotnet
                WellknownValue.DotnetExecutablePath => _dotnetEnvironment.Path,
                WellknownValue.DotnetLoggerDirectory => _environment.GetPath(SpecialFolder.Bin),
                WellknownValue.TeamCityMessagesPath => _teamCitySettings.ServiceMessagesPath,
                WellknownValue.DockerExecutablePath => _dockerEnvironment.Path,
                _ => string.Empty
            };
    }   
}