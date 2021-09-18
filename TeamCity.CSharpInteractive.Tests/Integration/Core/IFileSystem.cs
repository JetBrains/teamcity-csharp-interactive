namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System.Collections.Generic;

    internal interface IFileSystem
    {
        string CreateTempFilePath();
        
        void DeleteFile(string file);
        
        void AppendAllLines(string file, IEnumerable<string> lines);
    }
}