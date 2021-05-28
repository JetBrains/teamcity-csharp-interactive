// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    internal class FileTextReader : IFileTextReader
    {
        public string Read(string fileName) => File.ReadAllText(fileName);
    }
}