namespace Teamcity.Host
{
    internal class HostEnvironment : IHostEnvironment
    {
        public string? GetEnvironmentVariable(string name) => System.Environment.GetEnvironmentVariable(name);
    }
}