namespace TeamCity.CSharpInteractive;

using Script.Cmd;
using Script.DotNet;

internal interface IBuildMessagesProcessor
{
    void ProcessMessages(in Output output, IEnumerable<BuildMessage> messages, Action<BuildMessage> nextHandler);
}