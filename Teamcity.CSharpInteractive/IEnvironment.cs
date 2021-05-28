namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface IEnvironment
    {
        IEnumerable<string> GetCommandLineArgs();
    }
}