namespace TeamCity.CSharpInteractive.Tests;

using CSharpInteractive;
using Script.Cmd;
using Script.DotNet;

public class CustomMessagesProcessorTests
{
    private readonly Mock<IStartInfo> _startInfo = new();
    
    [Fact]
    public void ShouldProcessMessages()
    {
        // Given
        var output = new Output(_startInfo.Object, false, "Output", 11);
        var msg1 = new BuildMessage(BuildMessageState.StdOut, default, "Msg1");
        var msg2 = new BuildMessage(BuildMessageState.StdError, default, "Msg2");
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