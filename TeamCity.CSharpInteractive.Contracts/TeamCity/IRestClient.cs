// ReSharper disable CheckNamespace
namespace TeamCity
{
    using System.Threading.Tasks;

    public interface IRestClient
    {
        ValueTask<TResponse?> GetAsync<TResponse>(IRestRequest<TResponse> restRequest);
    }
}