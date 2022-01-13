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
        var messages = new BuildMessage[]
        {
            new(BuildMessageState.Info, default, "Msg1"),
            new(BuildMessageState.Error, default, "Msg2")
        };
        
        var nextHandler = new Mock<Action<Output>>();
        var processor = CreateInstance();

        // When
        processor.ProcessMessages(output, messages, nextHandler.Object);

        // Then
        nextHandler.Verify(i => i(new Output(_startInfo.Object, false, "Msg1")));
        nextHandler.Verify(i => i(new Output(_startInfo.Object, true, "Msg2")));
    }

    private static CustomMessagesProcessor CreateInstance() =>
        new();
}