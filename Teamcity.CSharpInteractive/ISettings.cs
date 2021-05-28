namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ISettings
    {
        string Title { get; }

        VerbosityLevel VerbosityLevel { get; set; }

        InteractionMode InteractionMode { get; }

        IEnumerable<ICodeSource> CodeSources { get; }

        void Load();
    }
}