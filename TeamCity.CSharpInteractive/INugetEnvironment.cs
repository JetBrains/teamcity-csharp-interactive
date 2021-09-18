// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface INugetEnvironment
    {
        IEnumerable<string> Sources { get; }
        
        IEnumerable<string> FallbackFolders { get; }

        string PackagesPath { get; }
    }
}