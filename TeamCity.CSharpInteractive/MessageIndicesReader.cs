// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Buffers;

internal class MessageIndicesReader : IMessageIndicesReader
{
    private readonly ILog<MessageIndicesReader> _log;
    private readonly MemoryPool<byte> _memoryPool;
    private readonly IFileSystem _fileSystem;

    public MessageIndicesReader(
        ILog<MessageIndicesReader> log,
        MemoryPool<byte> memoryPool,
        IFileSystem fileSystem)
    {
        _log = log;
        _memoryPool = memoryPool;
        _fileSystem = fileSystem;
    }

    public IEnumerable<ulong> Read(string indicesFile)
    {
        using var reader = _fileSystem.OpenReader(indicesFile);
        using var bufferOwner = _memoryPool.Rent(sizeof(ulong));
        var buffer = bufferOwner.Memory[..sizeof(ulong)];
        int size;
        var prevIndex = 0UL;
        var number = 0UL;
        while ((size = reader.Read(buffer)) == sizeof(ulong))
        {
            buffer.Span.Reverse();
            var index = BitConverter.ToUInt64(buffer.Span);
            if (index <= prevIndex)
            {
                _log.Warning($"Corrupted file \"{indicesFile}\", invalid index {index} at offset {number * sizeof(ulong)}.");
                break;
            }

            prevIndex = index;
            number++;
            yield return index;
        }

        if (size != 0)
        {
            _log.Warning($"Corrupted file \"{indicesFile}\", invalid size.");
        }
    }
}