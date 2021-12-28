namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IMessageIndicesReader
    {
        IEnumerable<ulong> Read(string indicesFile);
    }
}