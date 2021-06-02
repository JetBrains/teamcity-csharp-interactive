// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface INugetEnvironment
    {
        string StoreFolder { get; }

        IEnumerable<string> Sources { get; }
        
        IEnumerable<string> FallbackFolders { get; }
    }
}