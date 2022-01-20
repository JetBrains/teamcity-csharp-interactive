namespace TeamCity.CSharpInteractive;

using System;
using System.Collections.Generic;
using Cmd;
using DotNet;

internal interface IBuildMessagesProcessor
{
    void ProcessMessages(in Output output, IEnumerable<BuildMessage> messages, Action<BuildMessage> nextHandler);
}