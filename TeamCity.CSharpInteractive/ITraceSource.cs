namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ITraceSource
    {
        IEnumerable<Text> Trace { get; }
    }
}