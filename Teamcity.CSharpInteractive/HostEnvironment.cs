namespace Teamcity.CSharpInteractive
{
    internal class HostEnvironment : IHostEnvironment
    {
        public string? GetEnvironmentVariable(string name) => System.Environment.GetEnvironmentVariable(name);
    }
}