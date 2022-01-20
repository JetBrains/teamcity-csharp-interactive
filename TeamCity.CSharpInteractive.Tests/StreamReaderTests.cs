namespace TeamCity.CSharpInteractive.Tests;

using System.Buffers;
using StreamReader = StreamReader;

public class StreamReaderTests
{
    [Fact]
    public void ShouldRead()
    {
        // Given
        var stream = new MemoryStream(new byte[] {1, 2, 3});
        var instance = CreateInstance(stream);
        var pool = MemoryPool<byte>.Shared;
        using var owner = pool.Rent(2);
        var buffer = owner.Memory[..2];

        // When
        instance.Read(buffer).ShouldBe(2);

        // Then
        buffer.Span[0].ShouldBe((byte)1);
        buffer.Span[1].ShouldBe((byte)2);
    }
    
    [Fact]
    public void ShouldReadAndChangePosition()
    {
        // Given
        var stream = new MemoryStream(new byte[] {1, 2, 3});
        var instance = CreateInstance(stream);
        var pool = MemoryPool<byte>.Shared;
        using var owner = pool.Rent(2);
        var buffer = owner.Memory[..2];

        // When
        instance.Read(buffer).ShouldBe(2);
        instance.Read(buffer).ShouldBe(1);

        // Then
        buffer.Span[0].ShouldBe((byte)3);
    }
    
    [Fact]
    public void ShouldReadToTheEnd()
    {
        // Given
        var stream = new MemoryStream(new byte[] {1, 2, 3});
        var instance = CreateInstance(stream);
        var pool = MemoryPool<byte>.Shared;
        using var owner = pool.Rent(4);
        var buffer = owner.Memory[..4];

        // When
        instance.Read(buffer).ShouldBe(3);

        // Then
        buffer.Span[0].ShouldBe((byte)1);
        buffer.Span[1].ShouldBe((byte)2);
        buffer.Span[2].ShouldBe((byte)3);
    }
    
    [Fact]
    public void ShouldReadWithOffset()
    {
        // Given
        var stream = new MemoryStream(new byte[] {1, 2, 3});
        var instance = CreateInstance(stream);
        var pool = MemoryPool<byte>.Shared;
        using var owner = pool.Rent(2);
        var buffer = owner.Memory[..2];

        // When
        instance.Read(buffer, 1).ShouldBe(2);

        // Then
        buffer.Span[0].ShouldBe((byte)2);
        buffer.Span[1].ShouldBe((byte)3);
    }
    
    [Fact]
    public void ShouldReadWithOffsetToTheEnd()
    {
        // Given
        var stream = new MemoryStream(new byte[] {1, 2, 3});
        var instance = CreateInstance(stream);
        var pool = MemoryPool<byte>.Shared;
        using var owner = pool.Rent(3);
        var buffer = owner.Memory[..3];

        // When
        instance.Read(buffer, 1).ShouldBe(2);

        // Then
        buffer.Span[0].ShouldBe((byte)2);
        buffer.Span[1].ShouldBe((byte)3);
    }
    
    [Fact]
    public void ShouldReadWithOffsetToChangeIt()
    {
        // Given
        var stream = new MemoryStream(new byte[] {1, 2, 3});
        var instance = CreateInstance(stream);
        var pool = MemoryPool<byte>.Shared;
        using var owner = pool.Rent(2);
        var buffer = owner.Memory[..2];

        // When
        instance.Read(buffer, 0).ShouldBe(2);
        instance.Read(buffer).ShouldBe(1);

        // Then
        buffer.Span[0].ShouldBe((byte)3);
    }

    private static StreamReader CreateInstance(Stream stream) =>
        new(stream);
}