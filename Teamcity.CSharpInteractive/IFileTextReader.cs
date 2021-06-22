namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IFileTextReader
    {
        string Read(string fileName);

        IEnumerable<string> ReadLines(string fileName);
    }
}