// ReSharper disable CheckNamespace
namespace TeamCity
{
    public interface ITeamCity
    {
        IRestClient CreateClient(Endpoint endpoint);
    }
}