namespace TeamCity.CSharpInteractive.Tests;

using System;
using Cmd;
using Dotnet;
using Moq;
using Xunit;

public class CustomMessagesProcessorTests
{
    private readonly Mock<IStartInfo> _startInfo = new();
    
    [Fact]
    public void ShouldProcessMessages()
    {
        // Given
        var output = new Output(_startInfo.Object, false, "Output");
        var msg1 = new BuildMessage(BuildMessageState.Info, default, "Msg1");
        var msg2 = new BuildMessage(BuildMessageState.Error, default, "Msg2");
        var messages = new[] { msg1, msg2 };
        var nextHandler = new Mock<Action<BuildMessage>>();
        var processor = CreateInstance();

        // When
        processor.ProcessMessages(output, messages, nextHandler.Object);

        // Then
        nextHandler.Verify(i => i(msg1));
        nextHandler.Verify(i => i(msg2));
    }

    private static CustomMessagesProcessor CreateInstance() =>
        new();
}