namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using Host;

    internal interface ITraceSource
    {
        IEnumerable<Text> GetTrace();
    }
}