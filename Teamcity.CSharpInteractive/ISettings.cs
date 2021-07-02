namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ISettings
    {
        VerbosityLevel VerbosityLevel { get; set; }

        InteractionMode InteractionMode { get; }
        
        IEnumerable<ICodeSource> Sources { get; }
    }
}