namespace TeamCity.CSharpInteractive;

using System;

internal interface IEncoding
{
    string GetString(Memory<byte> buffer);
}