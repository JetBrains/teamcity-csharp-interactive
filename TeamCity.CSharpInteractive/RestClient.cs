namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class RestClient : IRestClient
    {
        private readonly Endpoint _endpoint;

        public RestClient(Endpoint endpoint)
        {
            if (endpoint.RequestUri == null) throw new ArgumentException("Request uri must be specified.", nameof(endpoint));
            _endpoint = endpoint;
        }

        public async ValueTask<TResponse?> GetAsync<TResponse>(IRestRequest<TResponse> restRequest)
        {
            if (restRequest.RelativeUri.IsAbsoluteUri) throw new ArgumentException("Uri must be relative.", nameof(restRequest));
            using var client = CreateClient();
            var stream = await client.GetStreamAsync(new Uri(_endpoint.RequestUri!, restRequest.RelativeUri));
            return await JsonSerializer.DeserializeAsync<TResponse>(stream);
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var headers = client.DefaultRequestHeaders;
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Add("Authorization", $"Bearer {_endpoint.AccessToken}");
            return client;
        }
    }
}