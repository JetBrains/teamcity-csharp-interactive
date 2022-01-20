// ReSharper disable CheckNamespace
namespace DotNet
{
    internal interface ISettings
    {
        bool LoggersAreRequired { get; }
        
        string DotNetExecutablePath { get; }

        string DotNetLoggerDirectory  { get; }

        string TeamCityMessagesPath  { get; }
    }
}