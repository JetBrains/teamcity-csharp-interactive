namespace TeamCity.CSharpInteractive;

using System;

public interface IFileReader : IDisposable
{
    int Read(Span<byte> buffer);

    int Read(Span<byte> buffer, long offset);
}