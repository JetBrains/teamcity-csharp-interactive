namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;

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
            var size = 0;
            while ((size = reader.Read(buffer.Span)) == sizeof(ulong))
            {
                var bytes = buffer.ToArray();
                Array.Reverse(bytes);
                var val = BitConverter.ToUInt64(bytes);
                yield return val;
            }

            if (size != 0)
            {
                _log.Warning($"Corrupted file \"{indicesFile}\", invalid size.");
            }
        }
    }
}