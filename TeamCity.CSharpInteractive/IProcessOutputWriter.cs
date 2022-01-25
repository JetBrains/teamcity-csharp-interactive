namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface IProcessOutputWriter
{
    void Write(in Output output);
}