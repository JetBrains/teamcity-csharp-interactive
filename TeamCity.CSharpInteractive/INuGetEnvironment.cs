// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive;

internal interface INuGetEnvironment
{
    IEnumerable<string> Sources { get; }
        
    IEnumerable<string> FallbackFolders { get; }

    string PackagesPath { get; }
}