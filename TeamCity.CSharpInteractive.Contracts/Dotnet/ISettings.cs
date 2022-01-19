// ReSharper disable CheckNamespace
namespace Dotnet
{
    internal interface ISettings
    {
        bool LoggersAreRequired { get; }
        
        string DotnetExecutablePath { get; }

        string DotnetLoggerDirectory  { get; }

        string TeamCityMessagesPath  { get; }
    }
}