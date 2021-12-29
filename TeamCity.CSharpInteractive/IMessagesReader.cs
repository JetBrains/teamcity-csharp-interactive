namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IMessagesReader
    {
        IEnumerable<IServiceMessage> Read(string indicesFile, string messagesFile);
    }
}