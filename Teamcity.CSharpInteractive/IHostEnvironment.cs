namespace Teamcity.CSharpInteractive
{
    internal interface IHostEnvironment
    {
        string? GetEnvironmentVariable(string name);
    }
}