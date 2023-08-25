namespace HostApi.DotNet;

using System.Text;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Read;
using JetBrains.TeamCity.ServiceMessages.Write;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using TeamCity.CSharpInteractive;

internal class DotNetTestReportingService : IDotNetTestReportingService
{
    private readonly IDotNetSettings _settings;
    private readonly ITeamCityWriter _teamCityWriter;
    private readonly IServiceMessageParser _serviceMessageParser;
    private readonly IFileSystem _fileSystem;

    public DotNetTestReportingService(
        IDotNetSettings settings,
        ITeamCityWriter teamCityWriter,
        IServiceMessageParser serviceMessageParser,
        IFileSystem fileSystem)
    {
        _settings = settings;
        _teamCityWriter = teamCityWriter;
        _serviceMessageParser = serviceMessageParser;
        _fileSystem = fileSystem;
    }

    public void SendTestResultsStreamingDataMessageIfNeeded()
    {
        var shouldStreamTestReportsFromFiles = !string.IsNullOrWhiteSpace(_settings.TeamCityTestReportFilesPathEnvValue);
        if (shouldStreamTestReportsFromFiles)
        {
            var importDataServiceMessage = new ServiceMessage("importData")
            {
                {"type", "streamToBuildLog"},
                {"filePattern", $"{_settings.TeamCityTestReportFilesPathEnvValue}/*.msg"},
                {"wrapFileContentInBlock", "false"}
            };
            _teamCityWriter.WriteRawMessage(importDataServiceMessage);
        }
    }

    public IEnumerable<IServiceMessage> GetServiceMessagesFromFilesWithTestReports()
    {
        var teamCityTestReportFilesPath = _settings.TeamCityTestReportFilesPathEnvValue;
        if (teamCityTestReportFilesPath == null || !_fileSystem.IsDirectoryExist(teamCityTestReportFilesPath))
        {
            yield break;
        }

        foreach (var fileWithTestReports in _fileSystem.EnumerateFiles(teamCityTestReportFilesPath, "*.msg", SearchOption.TopDirectoryOnly))
        {
            using var textReader = _fileSystem.OpenTextReader(fileWithTestReports, Encoding.UTF8);
            foreach (var serviceMessage in _serviceMessageParser.ParseServiceMessages(textReader))
            {
                yield return serviceMessage;
            }
        }
    }
}
