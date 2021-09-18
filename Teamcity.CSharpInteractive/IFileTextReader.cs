namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IFileTextReader
    {
        IEnumerable<string> ReadLines(string fileName);
    }
}