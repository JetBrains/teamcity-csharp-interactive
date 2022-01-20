// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Buffers;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Read;

internal class MessagesReader : IMessagesReader
{
    private readonly ILog<MessagesReader> _log;
    private readonly MemoryPool<byte> _memoryPool;
    private readonly IMessageIndicesReader _indicesReader;
    private readonly IFileSystem _fileSystem;
    private readonly IEncoding _encoding;
    private readonly IServiceMessageParser _serviceMessageParser;

    public MessagesReader(
        ILog<MessagesReader> log,
        MemoryPool<byte> memoryPool,
        IMessageIndicesReader indicesReader,
        IFileSystem fileSystem,
        IEncoding encoding,
        IServiceMessageParser serviceMessageParser)
    {
        _log = log;
        _memoryPool = memoryPool;
        _indicesReader = indicesReader;
        _fileSystem = fileSystem;
        _encoding = encoding;
        _serviceMessageParser = serviceMessageParser;
    }

    public IEnumerable<IServiceMessage> Read(string indicesFile, string messagesFile)
    {
        using var reader = _fileSystem.OpenReader(messagesFile);
        var position = 0UL;
        foreach (var index in _indicesReader.Read(indicesFile))
        {
            var size = (int)(index - position);
            if (size <= 0)
            {
                _log.Warning($"Corrupted file \"{indicesFile}\", invalid index {index}.");
                break;
            }
                
            using var owner = _memoryPool.Rent(size);
            var buffer = owner.Memory[..size];
            if (reader.Read(buffer, (long)position) != size)
            {
                _log.Warning($"Corrupted file \"{messagesFile}\", invalid size.");
                break;
            }

            position = index;
            var line = _encoding.GetString(buffer);
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            foreach (var message in _serviceMessageParser.ParseServiceMessages(line))
            {
                yield return message;
            }
        }
    }
}