// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System;
using System.Collections.Generic;
using Cmd;
using Dotnet;

internal class CustomMessagesProcessor : IBuildMessagesProcessor
{
    public void ProcessMessages(in Output output, IEnumerable<BuildMessage> messages, Action<Output> nextHandler)
    {
        foreach (var buildMessage in messages)
        {
            nextHandler(new Output(output.StartInfo, buildMessage.State > BuildMessageState.Warning, buildMessage.Text));
        }
    }
}