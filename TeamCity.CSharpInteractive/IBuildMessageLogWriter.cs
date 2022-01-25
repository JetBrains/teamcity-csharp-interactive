namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface IBuildMessageLogWriter
{
    void Write(BuildMessage message);
}