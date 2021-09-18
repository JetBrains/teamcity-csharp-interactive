namespace TeamCity.CSharpInteractive
{
    internal interface IHostEnvironment
    {
        string? GetEnvironmentVariable(string name);
    }
}