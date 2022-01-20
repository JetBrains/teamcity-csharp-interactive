namespace TeamCity.CSharpInteractive;

using Cmd;
using CSharpInteractive;
using DotNet;

internal interface IBuildMessagesProcessor
{
    void ProcessMessages(in Output output, IEnumerable<BuildMessage> messages, Action<BuildMessage> nextHandler);
}