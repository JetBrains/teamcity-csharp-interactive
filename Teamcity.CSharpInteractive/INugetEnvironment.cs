// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface INugetEnvironment
    {
        IEnumerable<string> Sources { get; }
        
        IEnumerable<string> FallbackFolders { get; }
    }
}