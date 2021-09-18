// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    internal class FileTextReader : IFileTextReader
    {
        public IEnumerable<string> ReadLines(string fileName) => File.ReadLines(fileName);
    }
}