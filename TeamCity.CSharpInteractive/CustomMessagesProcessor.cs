// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System;
using System.Collections.Generic;
using Cmd;
using DotNet;

internal class CustomMessagesProcessor : IBuildMessagesProcessor
{
    public void ProcessMessages(in Output output, IEnumerable<BuildMessage> messages, Action<BuildMessage> nextHandler)
    {
        foreach (var buildMessage in messages)
        {
            nextHandler(buildMessage);
        }
    }
}