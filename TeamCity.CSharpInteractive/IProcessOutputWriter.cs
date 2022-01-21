namespace TeamCity.CSharpInteractive;

using Script.Cmd;

internal interface IProcessOutputWriter
{
    void Write(in Output output);
}