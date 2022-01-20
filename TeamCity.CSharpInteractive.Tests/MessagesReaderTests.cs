namespace TeamCity.CSharpInteractive.Tests;

using System.Buffers;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Read;
using JetBrains.TeamCity.ServiceMessages.Write;

public class MessagesReaderTests
{
    private readonly Mock<ILog<MessagesReader>> _log = new();
    private readonly Mock<IMessageIndicesReader> _indicesReader = new();
    private readonly Mock<IFileSystem> _fileSystem = new();
    private readonly Mock<IEncoding> _encoding = new();
    private readonly Mock<IServiceMessageParser> _serviceMessageParser = new();

    [Fact]
    public void ShouldReadMessages()
    {
        // Given
        var streamReader = new Mock<IStreamReader>();
        
        using var bufferOwner1 = MemoryPool<byte>.Shared.Rent(4);
        var buffer1 = bufferOwner1.Memory[..3];
        buffer1.Span[0] = 1;
        var msg11 = new ServiceMessage("11");
        var msg12 = new ServiceMessage("12");
        var msg2 = new ServiceMessage("2");
        
        using var bufferOwner2 = MemoryPool<byte>.Shared.Rent(20 - 4);
        var buffer2 = bufferOwner2.Memory[..(20 - 4)];
        buffer2.Span[0] = 2;

        _fileSystem.Setup(i => i.OpenReader("data.msg")).Returns(streamReader.Object);
        _indicesReader.Setup(i => i.Read("data")).Returns(new [] { 4UL, 20UL });
       
        streamReader.Setup(i => i.Read(It.Is<Memory<byte>>(buffer => buffer.Length == 4), 0))
            .Callback<Memory<byte>, long>((buffer, _) => { buffer1.CopyTo(buffer); }).Returns(4);
        
        streamReader.Setup(i => i.Read(It.Is<Memory<byte>>(buffer => buffer.Length == 20 - 4), 4))
            .Callback<Memory<byte>, long>((buffer, _) => { buffer2.CopyTo(buffer); }).Returns(20 - 4);
        
        _encoding.Setup(i => i.GetString(It.Is<Memory<byte>>(buffer => buffer.ToArray()[0] == 1))).Returns("msg1");
        _encoding.Setup(i => i.GetString(It.Is<Memory<byte>>(buffer => buffer.ToArray()[0] == 2))).Returns("msg2");
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("msg1")).Returns(new [] { msg11, msg12 });
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("msg2")).Returns(new [] { msg12 });
        
        var reader = CreateInstance();

        // When
        var actualMessages = reader.Read("data", "data.msg").ToArray();
        
        // Then
        actualMessages.ShouldBe(new IServiceMessage[] { msg11, msg12, msg2 });
    }
    
    [Fact]
    public void ShouldReadMessagesWhenIndicesAreEmpty()
    {
        // Given
        var streamReader = new Mock<IStreamReader>();
        
        _fileSystem.Setup(i => i.OpenReader("data.msg")).Returns(streamReader.Object);
        _indicesReader.Setup(i => i.Read("data")).Returns(Array.Empty<ulong>());
       
        var reader = CreateInstance();

        // When
        var actualMessages = reader.Read("data", "data.msg").ToArray();
        
        // Then
        actualMessages.ShouldBe(Array.Empty<IServiceMessage>());
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldReadMessagesWhenLineIsEmpty(string line)
    {
        // Given
        var streamReader = new Mock<IStreamReader>();
        
        using var bufferOwner1 = MemoryPool<byte>.Shared.Rent(4);
        var buffer1 = bufferOwner1.Memory[..3];
        buffer1.Span[0] = 1;
        var msg11 = new ServiceMessage("11");
        var msg12 = new ServiceMessage("12");
        var msg2 = new ServiceMessage("2");
        
        using var bufferOwner2 = MemoryPool<byte>.Shared.Rent(20 - 4);
        var buffer2 = bufferOwner2.Memory[..(20 - 4)];
        buffer2.Span[0] = 2;

        _fileSystem.Setup(i => i.OpenReader("data.msg")).Returns(streamReader.Object);
        _indicesReader.Setup(i => i.Read("data")).Returns(new [] { 4UL, 20UL });
       
        streamReader.Setup(i => i.Read(It.Is<Memory<byte>>(buffer => buffer.Length == 4), 0))
            .Callback<Memory<byte>, long>((buffer, _) => { buffer1.CopyTo(buffer); }).Returns(4);
        
        streamReader.Setup(i => i.Read(It.Is<Memory<byte>>(buffer => buffer.Length == 20 - 4), 4))
            .Callback<Memory<byte>, long>((buffer, _) => { buffer2.CopyTo(buffer); }).Returns(20 - 4);
        
        _encoding.Setup(i => i.GetString(It.Is<Memory<byte>>(buffer => buffer.ToArray()[0] == 1))).Returns(line);
        _encoding.Setup(i => i.GetString(It.Is<Memory<byte>>(buffer => buffer.ToArray()[0] == 2))).Returns("msg2");
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("msg1")).Returns(new [] { msg11, msg12 });
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("msg2")).Returns(new [] { msg12 });
        
        var reader = CreateInstance();

        // When
        var actualMessages = reader.Read("data", "data.msg").ToArray();
        
        // Then
        actualMessages.ShouldBe(new IServiceMessage[] { msg2 });
    }
    
    [Fact]
    public void ShouldReadMessagesWhenMessagesFileIsCorrupted()
    {
        // Given
        var streamReader = new Mock<IStreamReader>();
        
        using var bufferOwner1 = MemoryPool<byte>.Shared.Rent(4);
        var buffer1 = bufferOwner1.Memory[..3];
        buffer1.Span[0] = 1;
        var msg11 = new ServiceMessage("11");
        var msg12 = new ServiceMessage("12");

        using var bufferOwner2 = MemoryPool<byte>.Shared.Rent(20 - 4);
        var buffer2 = bufferOwner2.Memory[..(20 - 4)];
        buffer2.Span[0] = 2;

        _fileSystem.Setup(i => i.OpenReader("data.msg")).Returns(streamReader.Object);
        _indicesReader.Setup(i => i.Read("data")).Returns(new [] { 4UL, 20UL });
       
        streamReader.Setup(i => i.Read(It.Is<Memory<byte>>(buffer => buffer.Length == 4), 0))
            .Callback<Memory<byte>, long>((buffer, _) => { buffer1.CopyTo(buffer); }).Returns(4);
        
        streamReader.Setup(i => i.Read(It.Is<Memory<byte>>(buffer => buffer.Length == 20 - 4), 4))
            .Callback<Memory<byte>, long>((buffer, _) => { buffer2.CopyTo(buffer); }).Returns(20 - 8);
        
        _encoding.Setup(i => i.GetString(It.Is<Memory<byte>>(buffer => buffer.ToArray()[0] == 1))).Returns("msg1");
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("msg1")).Returns(new [] { msg11, msg12 });

        var reader = CreateInstance();

        // When
        var actualMessages = reader.Read("data", "data.msg").ToArray();
        
        // Then
        _log.Verify(i => i.Warning(It.Is<Text[]>(warning => warning.Single().Value.Contains("invalid size"))));
        actualMessages.ShouldBe(new IServiceMessage[] { msg11, msg12 });
    }
    
    [Fact]
    public void ShouldReadMessagesWhenInvalidIndex()
    {
        // Given
        var streamReader = new Mock<IStreamReader>();
        
        using var bufferOwner1 = MemoryPool<byte>.Shared.Rent(4);
        var buffer1 = bufferOwner1.Memory[..3];
        buffer1.Span[0] = 1;
        var msg11 = new ServiceMessage("11");
        var msg12 = new ServiceMessage("12");
        
        _fileSystem.Setup(i => i.OpenReader("data.msg")).Returns(streamReader.Object);
        _indicesReader.Setup(i => i.Read("data")).Returns(new [] { 4UL, 2UL });
       
        streamReader.Setup(i => i.Read(It.Is<Memory<byte>>(buffer => buffer.Length == 4), 0))
            .Callback<Memory<byte>, long>((buffer, _) => { buffer1.CopyTo(buffer); }).Returns(4);
        
        _encoding.Setup(i => i.GetString(It.Is<Memory<byte>>(buffer => buffer.ToArray()[0] == 1))).Returns("msg1");
        _serviceMessageParser.Setup(i => i.ParseServiceMessages("msg1")).Returns(new [] { msg11, msg12 });
        
        var reader = CreateInstance();

        // When
        var actualMessages = reader.Read("data", "data.msg").ToArray();
        
        // Then
        _log.Verify(i => i.Warning(It.Is<Text[]>(warning => warning.Single().Value.Contains("invalid index"))));
        actualMessages.ShouldBe(new IServiceMessage[] { msg11, msg12 });
    }

    private MessagesReader CreateInstance() =>
        new(_log.Object, MemoryPool<byte>.Shared, _indicesReader.Object, _fileSystem.Object, _encoding.Object, _serviceMessageParser.Object);
}