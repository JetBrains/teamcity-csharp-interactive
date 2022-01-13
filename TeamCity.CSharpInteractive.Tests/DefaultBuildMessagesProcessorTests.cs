namespace TeamCity.CSharpInteractive.Tests;

using System;
using Cmd;
using Dotnet;
using JetBrains.TeamCity.ServiceMessages;
using Moq;
using Xunit;

public class DefaultBuildMessagesProcessorTests
{
    private readonly Mock<ITeamCitySettings> _teamCitySettings = new();
    private readonly Mock<IProcessOutputWriter> _processOutputWriter = new();
    private readonly Mock<IBuildMessageLogWriter> _buildMessageLogWriter = new();
    private readonly Mock<IStartInfo> _startInfo = new();

    [Fact]
    public void ShouldSendServiceMessagesToTeamCityViaProcessOutputWhenIsUnderTeamCityAndHasNayServiceMessage()
    {
        // Given
        var output = new Output(_startInfo.Object, false, "Output");
        var messages = new BuildMessage[]
        {
            new(BuildMessageState.Info, default, "Msg1"),
            new(BuildMessageState.ServiceMessage, Mock.Of<IServiceMessage?>())
        };

        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(true);
        var nextHandler = new Mock<Action<Output>>();
        var processor = CreateInstance();

        // When
        processor.ProcessMessages(output, messages, nextHandler.Object);

        // Then
        _processOutputWriter.Verify(i => i.Write(output));
        nextHandler.Verify(i => i(It.IsAny<Output>()), Times.Never);
    }
    
    [Fact]
    public void ShouldProcessBuildMessageWhenIsNotUnderTeamCity()
    {
        // Given
        var output = new Output(_startInfo.Object, false, "Output");
        var msg1 = new BuildMessage(BuildMessageState.Info, default, "Msg1");
        var msg2 = new BuildMessage(BuildMessageState.ServiceMessage, Mock.Of<IServiceMessage?>());

        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(false);
        var nextHandler = new Mock<Action<Output>>();
        var processor = CreateInstance();

        // When
        processor.ProcessMessages(output, new[] { msg1, msg2 }, nextHandler.Object);

        // Then
        _buildMessageLogWriter.Verify(i => i.Write(msg1));
        _buildMessageLogWriter.Verify(i => i.Write(msg2));
        _processOutputWriter.Verify(i => i.Write(It.IsAny<Output>()), Times.Never);
        nextHandler.Verify(i => i(It.IsAny<Output>()), Times.Never);
    }
    
    [Fact]
    public void ShouldProcessBuildMessageWhenHasNotTeamCityServiceMessages()
    {
        // Given
        var output = new Output(_startInfo.Object, false, "Output");
        var msg1 = new BuildMessage(BuildMessageState.Info, default, "Msg1");
        var msg2 = new BuildMessage(BuildMessageState.Error, default, "Error");

        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(true);
        var nextHandler = new Mock<Action<Output>>();
        var processor = CreateInstance();

        // When
        processor.ProcessMessages(output, new[] { msg1, msg2 }, nextHandler.Object);

        // Then
        _buildMessageLogWriter.Verify(i => i.Write(msg1));
        _buildMessageLogWriter.Verify(i => i.Write(msg2));
        _processOutputWriter.Verify(i => i.Write(It.IsAny<Output>()), Times.Never);
        nextHandler.Verify(i => i(It.IsAny<Output>()), Times.Never);
    }

    private DefaultBuildMessagesProcessor CreateInstance() =>
        new(_teamCitySettings.Object, _processOutputWriter.Object, _buildMessageLogWriter.Object);
}