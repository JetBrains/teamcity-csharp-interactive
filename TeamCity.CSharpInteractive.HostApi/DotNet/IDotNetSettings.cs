// ReSharper disable InconsistentNaming
namespace HostApi.DotNet;

internal interface IDotNetSettings
{
    bool LoggersAreRequired { get; }

    string DotNetExecutablePath { get; }

    string DotNetMSBuildLoggerDirectory { get; }

    string DotNetVSTestLoggerDirectory { get; }

    string? TeamCityServiceMessagesBackupPathEnvValue { get; }

    string? TeamCityTestReportFilesPathEnvValue { get; }

    string? TeamCityFallbackToStdOutTestReportingEnvValue { get; }
}
