namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IMessagesReader
    {
        IEnumerable<string> Read(string indicesFile, string messagesFile);
    }
}