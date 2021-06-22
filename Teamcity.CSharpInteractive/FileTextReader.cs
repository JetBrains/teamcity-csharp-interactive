// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    internal class FileTextReader : IFileTextReader
    {
        public string Read(string fileName) => File.ReadAllText(fileName);
        
        public IEnumerable<string> ReadLines(string fileName) => File.ReadLines(fileName);
    }
}