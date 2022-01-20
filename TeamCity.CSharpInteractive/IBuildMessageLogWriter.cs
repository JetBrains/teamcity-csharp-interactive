namespace TeamCity.CSharpInteractive;

using DotNet;

internal interface IBuildMessageLogWriter
{
    void Write(BuildMessage message);
}