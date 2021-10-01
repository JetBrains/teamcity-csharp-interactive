// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    internal interface IEnvironmentVariables
    {
        string? GetEnvironmentVariable(string variable);
    }
}