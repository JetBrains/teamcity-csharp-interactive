namespace TeamCity.CSharpInteractive
{
    using System;
    using System.IO;

    internal class StreamReader: IStreamReader
    {
        private readonly object _lockObject = new();
        private readonly Stream _stream;

        public StreamReader(Stream stream) => _stream = stream;

        public int Read(Memory<byte> buffer)
        {
            lock (_lockObject)
            {
                return _stream.Read(buffer.Span);
            }
        }

        public int Read(Memory<byte> buffer, long offset)
        {
            lock (_lockObject)
            {
                _stream.Seek(offset, SeekOrigin.Begin);
                return _stream.Read(buffer.Span);
            }
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                _stream.Dispose();
            }
            
            GC.SuppressFinalize(this);
        }
    }
}