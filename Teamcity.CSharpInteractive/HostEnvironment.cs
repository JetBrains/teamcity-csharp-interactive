// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class HostEnvironment : IHostEnvironment
    {
        public string? GetEnvironmentVariable(string name) => System.Environment.GetEnvironmentVariable(name);
    }
}