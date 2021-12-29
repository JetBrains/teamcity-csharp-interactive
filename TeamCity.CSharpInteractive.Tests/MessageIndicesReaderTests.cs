namespace TeamCity.CSharpInteractive.Tests;

using System;
using System.Buffers;
using System.IO;
using System.Linq;
using Moq;
using Shouldly;
using Xunit;
using StreamReader = StreamReader;

public class MessageIndicesReaderTests
{
    private readonly Mock<ILog<MessageIndicesReader>> _log = new();
    private readonly Mock<IFileSystem> _fileSystem = new();

    [Fact]
    public void ShouldReadIndices()
    {
        // Given
        var reader = CreateInstance();
        using var ms = new MemoryStream();
        Write(ms, 1UL);
        Write(ms, 123456789UL);
        ms.Seek(0, SeekOrigin.Begin);
        _fileSystem.Setup(i => i.OpenReader("data")).Returns(new StreamReader(ms));

        // When
        var expectedIndices = reader.Read("data").ToArray();

        // Then
        expectedIndices.ShouldBe(new []{ 1UL, 123456789UL });
    }
    
    [Fact]
    public void ShouldReadIndicesWhenEmpty()
    {
        // Given
        var reader = CreateInstance();
        using var ms = new MemoryStream();
        ms.Seek(0, SeekOrigin.Begin);
        _fileSystem.Setup(i => i.OpenReader("data")).Returns(new StreamReader(ms));

        // When
        var expectedIndices = reader.Read("data").ToArray();

        // Then
        expectedIndices.ShouldBe(Array.Empty<ulong>());
    }
    
    [Fact]
    public void ShouldShowWarningWhenCorrupted()
    {
        // Given
        var reader = CreateInstance();
        using var ms = new MemoryStream();
        Write(ms, 1UL);
        Write(ms, 123456789UL);
        ms.Write(BitConverter.GetBytes((byte)12));
        ms.Seek(0, SeekOrigin.Begin);
        _fileSystem.Setup(i => i.OpenReader("data")).Returns(new StreamReader(ms));

        // When
        var expectedIndices = reader.Read("data").ToArray();

        // Then
        _log.Verify(i => i.Warning(It.Is<Text[]>(warning => warning.Single().Value.Contains("invalid size"))));
        expectedIndices.ShouldBe(new []{ 1UL, 123456789UL });
    }
    
    [Fact]
    public void ShouldShowWarningWhenInvalidValues()
    {
        // Given
        var reader = CreateInstance();
        using var ms = new MemoryStream();
        Write(ms, 123UL);
        Write(ms, 1UL);
        Write(ms, 1234UL);
        ms.Seek(0, SeekOrigin.Begin);
        _fileSystem.Setup(i => i.OpenReader("data")).Returns(new StreamReader(ms));

        // When
        var expectedIndices = reader.Read("data").ToArray();

        // Then
        _log.Verify(i => i.Warning(It.Is<Text[]>(warning => warning.Single().Value.Contains("invalid index"))));
        expectedIndices.ShouldBe(new []{ 123UL });
    }

    private static void Write(Stream stream, ulong index)
    {
        var bytes = BitConverter.GetBytes(index);
        Array.Reverse(bytes);
        stream.Write(bytes);
    }

    private MessageIndicesReader CreateInstance() =>
        new(_log.Object, MemoryPool<byte>.Shared, _fileSystem.Object);
}