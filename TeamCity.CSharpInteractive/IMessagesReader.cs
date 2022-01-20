namespace TeamCity.CSharpInteractive;

using JetBrains.TeamCity.ServiceMessages;

internal interface IMessagesReader
{
    IEnumerable<IServiceMessage> Read(string indicesFile, string messagesFile);
}