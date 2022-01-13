namespace TeamCity.CSharpInteractive.Tests;

using System.Linq;
using Cmd;
using Dotnet;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Read;
using Moq;
using Shouldly;
using Xunit;

public class BuildOutputConverterTests
{
    private readonly Mock<IServiceMessageParser> _serviceMessageParser = new();
    private readonly Mock<IStartInfo> _startInfo = new();
    private readonly Mock<IBuildResult> _buildResult = new();

    [Fact]
    public void ShouldConvertMessages()
    {
        // Given
        Mock<IServiceMessage> msg1 = new(); 
        Mock<IServiceMessage> msg2 = new();
        BuildMessage buildMsg1 = new();
        BuildMessage buildMsg2 = new();
        BuildMessage buildMsg3 = new();
        var messagesOutput = new Output(_startInfo.Object, false, "Messages");
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("Messages")).Returns(new [] { msg1.Object, msg2.Object });
        _buildResult.Setup(i => i.ProcessMessage(_startInfo.Object, msg1.Object)).Returns(new [] { buildMsg1, buildMsg2 });
        _buildResult.Setup(i => i.ProcessMessage(_startInfo.Object, msg2.Object)).Returns(new [] { buildMsg3 });
        var converter = CreateInstance();

        // When
        var messages = converter.Convert(messagesOutput, _buildResult.Object).ToArray();

        // Then
        messages.ShouldBe(
            new[]
            {
                new (BuildMessageState.ServiceMessage, msg1.Object),
                buildMsg1,
                buildMsg2,
                new (BuildMessageState.ServiceMessage, msg2.Object),
                buildMsg3
            });
    }
    
    [Theory]
    [InlineData(false, BuildMessageState.Info)]
    [InlineData(true, BuildMessageState.Error)]
    public void ShouldConvertToSimpleMessageWhenHasNoServiceMessages(bool isError, BuildMessageState state)
    {
        // Given
        var output = new Output(_startInfo.Object, isError, "some output");
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("some output")).Returns(Enumerable.Empty<IServiceMessage>());
        var converter = CreateInstance();

        // When
        var messages = converter.Convert(output, _buildResult.Object).ToArray();

        // Then
        messages.ShouldBe(new[] { new BuildMessage(state, default, output.Line) });
    }

    private BuildOutputConverter CreateInstance() =>
        new(_serviceMessageParser.Object);
}