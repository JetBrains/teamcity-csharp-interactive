namespace TeamCity.CSharpInteractive;

internal interface IStreamReader : IDisposable
{
    int Read(Memory<byte> buffer);

    int Read(Memory<byte> buffer, long offset);
}