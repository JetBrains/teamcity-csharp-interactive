namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IFileExplorer
    {
        IEnumerable<string> FindFiles(string searchPattern, params string[] additionalVariables);
    }
}