// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive.Tests.Integration.Core;

internal interface IFileSystem
{
    string CreateTempFilePath();
        
    void DeleteFile(string file);
        
    void AppendAllLines(string file, IEnumerable<string> lines);
}