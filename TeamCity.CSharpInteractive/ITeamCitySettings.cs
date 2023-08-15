// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive;

internal interface ITeamCitySettings
{
    bool IsUnderTeamCity { get; }

    string Version { get; }

    string FlowId { get; }

    string? ServiceMessagesBackupPathEnvValue { get; }

    string? TestReportFilesPathEnvValue { get; }

    string? FallbackToStdOutTestReportingEnvValue { get; }
}
