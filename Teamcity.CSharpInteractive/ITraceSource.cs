namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ITraceSource
    {
        IEnumerable<Text> GetTrace();
    }
}