namespace TeamCity.CSharpInteractive
{
    using System;
    using System.IO;

    public class StreamReader: IStreamReader
    {
        private readonly object _lockObject = new();
        private readonly Stream _stream;

        public StreamReader(Stream stream) => _stream = stream;

        public int Read(Span<byte> buffer)
        {
            lock (_lockObject)
            {
                return _stream.Read(buffer);
            }
        }

        public int Read(Span<byte> buffer, long offset)
        {
            lock (_lockObject)
            {
                _stream.Seek(offset, SeekOrigin.Begin);
                return _stream.Read(buffer);
            }
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                _stream.Dispose();
            }
        }
    }
}