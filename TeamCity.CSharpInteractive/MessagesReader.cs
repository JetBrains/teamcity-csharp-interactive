// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Buffers;
    using System.Collections.Generic;
    using System.Text;

    internal class MessagesReader : IMessagesReader
    {
        private readonly ILog<MessagesReader> _log;
        private readonly MemoryPool<byte> _memoryPool;
        private readonly IMessageIndicesReader _indicesReader;
        private readonly IFileSystem _fileSystem;

        public MessagesReader(
            ILog<MessagesReader> log,
            MemoryPool<byte> memoryPool,
            IMessageIndicesReader indicesReader,
            IFileSystem fileSystem)
        {
            _log = log;
            _memoryPool = memoryPool;
            _indicesReader = indicesReader;
            _fileSystem = fileSystem;
        }

        public IEnumerable<string> Read(string indicesFile, string messagesFile)
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
                var buffer = owner.Memory.Span[..size];
                if (reader.Read(buffer, (long)position) != size)
                {
                    _log.Warning($"Corrupted file \"{messagesFile}\", invalid size.");
                    break;
                }

                var line = Encoding.UTF8.GetString(buffer);
                yield return line;
                position = index;
            }
        }
    }
}