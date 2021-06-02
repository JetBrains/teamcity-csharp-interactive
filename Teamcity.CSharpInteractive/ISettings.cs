namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ISettings
    {
        string Title { get; }

        VerbosityLevel VerbosityLevel { get; set; }

        InteractionMode InteractionMode { get; }

        void Load();
        
        IEnumerable<ICodeSource> Sources { get; }
    }
}