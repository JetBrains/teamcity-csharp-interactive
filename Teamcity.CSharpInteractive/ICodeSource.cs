namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ICodeSource : IEnumerable<string>
    {
        string Name { get; }
        
        bool ResetRequired { get; }
    }
}