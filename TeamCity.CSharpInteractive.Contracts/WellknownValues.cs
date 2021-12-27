// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive.Contracts
{
    internal static class WellknownValues
    {
        // Dotnet
        public const string DotnetExecutablePath = "%DotnetExecutablePath%";
        public const string DotnetLoggerDirectory = "%DotnetLoggerDirectory%";
        public const string TeamCityVersion = "%TeamCityVersion%";
        public const string TeamCityMessagesPath = "%TeamCityMessagesPath%";
        // Docker
        public const string DockerExecutablePath = "%DockerExecutablePath%";
    }
}