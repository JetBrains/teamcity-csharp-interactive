// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class TeamCitySettings : ITeamCitySettings
{
    private const string VersionVariableName = "TEAMCITY_VERSION";
    private const string ProjectNameVariableName = "TEAMCITY_PROJECT_NAME";
    internal const string FlowIdEnvironmentVariableName = "TEAMCITY_PROCESS_FLOW_ID";
    private const string ServiceMessagesBackupPathEnvironmentVariableName = "TEAMCITY_SERVICE_MESSAGES_PATH";
    private const string TestReportFilesPathEnvironmentVariableName = "TEAMCITY_TEST_REPORT_FILES_PATH";
    private const string FallbackToStdOutTestReportingEnvironmentVariableName = "TEAMCITY_FALLBACK_TO_STDOUT_TEST_REPORTING";
    private const string DefaultFlowId = "ROOT";
    private readonly IHostEnvironment _hostEnvironment;
    private readonly Lazy<bool> _isUnderTeamCity;
    private readonly Lazy<string> _flowId;
    private readonly Lazy<string?> _serviceMessagesBackupPathEnvValue;
    private readonly Lazy<string?> _testReportFilesPathEnvValue;
    private readonly Lazy<string?> _fallbackToStdOutTestReportingEnvValue;

    public TeamCitySettings(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
        _isUnderTeamCity = new Lazy<bool>(() =>
            !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable(ProjectNameVariableName))
            || !string.IsNullOrWhiteSpace(_hostEnvironment.GetEnvironmentVariable(VersionVariableName)));

        _flowId = new Lazy<string>(() =>
        {
            var flowId = _hostEnvironment.GetEnvironmentVariable(FlowIdEnvironmentVariableName);
            return string.IsNullOrWhiteSpace(flowId) ? DefaultFlowId : flowId;
        });

        _serviceMessagesBackupPathEnvValue = new Lazy<string?>(() => _hostEnvironment.GetEnvironmentVariable(ServiceMessagesBackupPathEnvironmentVariableName));
        _testReportFilesPathEnvValue = new Lazy<string?>(() => _hostEnvironment.GetEnvironmentVariable(TestReportFilesPathEnvironmentVariableName));
        _fallbackToStdOutTestReportingEnvValue = new Lazy<string?>(() => _hostEnvironment.GetEnvironmentVariable(FallbackToStdOutTestReportingEnvironmentVariableName));
    }

    public bool IsUnderTeamCity => _isUnderTeamCity.Value;

    public string Version => (_hostEnvironment.GetEnvironmentVariable(VersionVariableName) ?? string.Empty).Trim();

    public string FlowId => _flowId.Value;

    public string? ServiceMessagesBackupPathEnvValue => _serviceMessagesBackupPathEnvValue.Value;

    public string? TestReportFilesPathEnvValue => _testReportFilesPathEnvValue.Value;

    public string? FallbackToStdOutTestReportingEnvValue => _fallbackToStdOutTestReportingEnvValue.Value;
}
