namespace TeamCity.CSharpInteractive.Tests;

using System;
using Cmd;
using Moq;
using Xunit;

public class ProcessOutputWriterTests
{
    private readonly Mock<IConsole> _console = new();

    [Fact]
    public void ShouldWriteStdOut()
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new Output(Mock.Of<IStartInfo>(), false, "Out"));

        // Then
        _console.Verify(i => i.WriteToOut(It.Is<(ConsoleColor? color, string output)[]>(items => items.Length == 2 && items[0].output == "Out" && items[1].output == Environment.NewLine)));
    }
    
    [Fact]
    public void ShouldWriteStdErr()
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new Output(Mock.Of<IStartInfo>(), true, "Err"));

        // Then
        _console.Verify(i => i.WriteToErr("Err", Environment.NewLine));
    }

    private ProcessOutputWriter CreateInstance() =>
        new(_console.Object);
}