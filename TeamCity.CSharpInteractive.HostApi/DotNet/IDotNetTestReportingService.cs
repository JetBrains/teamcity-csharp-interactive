namespace HostApi.DotNet;

using JetBrains.TeamCity.ServiceMessages;

internal interface IDotNetTestReportingService
{
    void SendTestResultsStreamingDataMessageIfNeeded();

    IEnumerable<IServiceMessage> GetServiceMessagesFromFilesWithTestReports();
}