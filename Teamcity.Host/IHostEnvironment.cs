namespace Teamcity.Host
{
    internal interface IHostEnvironment
    {
        string? GetEnvironmentVariable(string name);
    }
}