namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface INugetAssetsReader
    {
        IEnumerable<ReferencingAssembly> ReadAssemblies(string assetsFilePath);
    }
}