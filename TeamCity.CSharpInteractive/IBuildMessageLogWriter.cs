namespace TeamCity.CSharpInteractive;

using Script.DotNet;

internal interface IBuildMessageLogWriter
{
    void Write(BuildMessage message);
}