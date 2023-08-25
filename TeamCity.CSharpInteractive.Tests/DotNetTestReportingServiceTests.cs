namespace TeamCity.CSharpInteractive.Tests;

using System.Reflection;
using System.Text;
using HostApi.DotNet;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Read;
using JetBrains.TeamCity.ServiceMessages.Write;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

public class DotNetTestReportingServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldNotSendTestResultsStreamingDataMessageIfTestReportsPathIsNotSet(string testReportFilesPath)
    {
        // Given
        Mock<IDotNetSettings> settingsMock = new();
        settingsMock.Setup(x => x.TeamCityTestReportFilesPathEnvValue).Returns(testReportFilesPath);
        Mock<ITeamCityWriter> teamCityWriterMock = new();
        Mock<IServiceMessageParser> serviceMessageParserMock = new();
        Mock<IFileSystem> fileSystemMock = new();
        var dotNetTestReportingService = new DotNetTestReportingService(
            settingsMock.Object,
            teamCityWriterMock.Object,
            serviceMessageParserMock.Object,
            fileSystemMock.Object);

        // When
        dotNetTestReportingService.SendTestResultsStreamingDataMessageIfNeeded();

        // Then
        teamCityWriterMock.Verify(x => x.WriteRawMessage(It.IsAny<IServiceMessage>()), Times.Never);
    }

    [Fact]
    public void ShouldSendTestResultsStreamingDataMessageIfTestReportsPathIsSet()
    {
        // Given
        var testReportFilesPath = "path";
        Mock<IDotNetSettings> settingsMock = new();
        settingsMock.Setup(x => x.TeamCityTestReportFilesPathEnvValue).Returns(testReportFilesPath);
        Mock<ITeamCityWriter> teamCityWriterMock = new();
        Mock<IServiceMessageParser> serviceMessageParserMock = new();
        Mock<IFileSystem> fileSystemMock = new();
        var dotNetTestReportingService = new DotNetTestReportingService(
            settingsMock.Object,
            teamCityWriterMock.Object,
            serviceMessageParserMock.Object,
            fileSystemMock.Object);

        // When
        dotNetTestReportingService.SendTestResultsStreamingDataMessageIfNeeded();

        // Then
        teamCityWriterMock.Verify(
            x => x.WriteRawMessage(
                It.Is<IServiceMessage>(m => m.Name == "importData"
                                            && m.GetValue("type") == "streamToBuildLog"
                                            && m.GetValue("filePattern") == "path/*.msg"
                                            && m.GetValue("wrapFileContentInBlock") == "false")));
    }

    [Fact]
    public void ShouldReturnEmptyServiceMessageEnumerableIfTestReportsDirectoryDoesNotExist()
    {
        // Given
        var testReportFilesPath = "path";
        Mock<IDotNetSettings> settingsMock = new();
        settingsMock.Setup(x => x.TeamCityTestReportFilesPathEnvValue).Returns(testReportFilesPath);
        Mock<ITeamCityWriter> teamCityWriterMock = new();
        Mock<IServiceMessageParser> serviceMessageParserMock = new();
        Mock<IFileSystem> fileSystemMock = new();
        fileSystemMock.Setup(x => x.IsDirectoryExist(testReportFilesPath)).Returns(false);
        var dotNetTestReportingService = new DotNetTestReportingService(
            settingsMock.Object,
            teamCityWriterMock.Object,
            serviceMessageParserMock.Object,
            fileSystemMock.Object);

        // When
        var serviceMessages = dotNetTestReportingService.GetServiceMessagesFromFilesWithTestReports();

        // Then
        Assert.Empty(serviceMessages);
    }

    [Fact]
    public void ShouldReturnTestReportingServiceMessagesFromFilesWithTestReports()
    {
        // Given
        var testReportFilesPath = "path";
        Mock<IDotNetSettings> settingsMock = new();
        settingsMock.Setup(x => x.TeamCityTestReportFilesPathEnvValue).Returns(testReportFilesPath);

        Mock<ITeamCityWriter> teamCityWriterMock = new();

        using var file1TextReader = new StringReader("##teamcity[testStarted name='test1']\n##teamcity[testFinished name='test1']");
        using var file2TextReader = new StringReader("##teamcity[testStarted name='test2']");
        Mock<IFileSystem> fileSystemMock = new();
        fileSystemMock.Setup(x => x.IsDirectoryExist(testReportFilesPath)).Returns(true);
        fileSystemMock.Setup(x => x.EnumerateFiles(testReportFilesPath, It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new[] {"1", "2"});
        fileSystemMock.Setup(x => x.OpenTextReader("1", It.IsAny<Encoding>())).Returns(file1TextReader);
        fileSystemMock.Setup(x => x.OpenTextReader("2", It.IsAny<Encoding>())).Returns(file2TextReader);

        var serviceMessage1 = new ServiceMessage("msg1");
        var serviceMessage2 = new ServiceMessage("msg2");
        var serviceMessage3 = new ServiceMessage("msg3");
        Mock<IServiceMessageParser> serviceMessageParserMock = new();
        serviceMessageParserMock
            .Setup(x => x.ParseServiceMessages(It.Is<TextReader>(r => r == file1TextReader)))
            .Returns(new[] {serviceMessage1, serviceMessage2});
        serviceMessageParserMock
            .Setup(x => x.ParseServiceMessages(It.Is<TextReader>(r => r == file2TextReader)))
            .Returns(new[] {serviceMessage3});

        var dotNetTestReportingService = new DotNetTestReportingService(
            settingsMock.Object,
            teamCityWriterMock.Object,
            serviceMessageParserMock.Object,
            fileSystemMock.Object);

        // When
        var serviceMessages = dotNetTestReportingService.GetServiceMessagesFromFilesWithTestReports().ToList();

        // Then
        Assert.Equal(3, serviceMessages.Count);
        Assert.Equal("msg1", serviceMessages[0].Name);
        Assert.Equal("msg2", serviceMessages[1].Name);
        Assert.Equal("msg3", serviceMessages[2].Name);
    }
}
