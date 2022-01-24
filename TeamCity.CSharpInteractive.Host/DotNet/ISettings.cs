// ReSharper disable InconsistentNaming
namespace Script.DotNet;

internal interface ISettings
{
    bool LoggersAreRequired { get; }
        
    string DotNetExecutablePath { get; }

    string DotNetMSBuildLoggerDirectory  { get; }
    
    string DotNetVSTestLoggerDirectory  { get; }

    string TeamCityMessagesPath  { get; }
}