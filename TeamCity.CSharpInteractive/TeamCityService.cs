// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;

    internal class TeamCityService : ITeamCity
    {
        private static readonly Uri DefaultServer = new("https://localhost/");
        private readonly ITeamCityParameters _teamCityParameters;
        private readonly Func<Endpoint, IRestClient> _clientFactory;

        public TeamCityService(
            ITeamCityParameters teamCityParameters,
            Func<Endpoint, IRestClient> clientFactory)
        {
            _teamCityParameters = teamCityParameters;
            _clientFactory = clientFactory;
        }

        public IRestClient CreateClient(Endpoint endpoint)
        {
            var requestUri = new Uri(endpoint.RequestUri ?? GetDefaultRequestUri(), "app/rest/");
            return _clientFactory(endpoint.WithRequestUri(requestUri));
        }

        private Uri GetDefaultRequestUri()
        {
            if (_teamCityParameters.TryGetParameter(TeamCityParameterType.Configuration, "ServerUrl", out var sererUrl)
                && Uri.TryCreate(sererUrl, UriKind.Absolute, out var sererUri))
            {
                return sererUri;
            }

            return DefaultServer;
        }
    }
}