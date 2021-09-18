namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ISettings
    {
        IReadOnlyList<string> ScriptArguments { get; }
        
        IReadOnlyDictionary<string, string> ScriptProperties { get; }
        
        VerbosityLevel VerbosityLevel { get; }

        InteractionMode InteractionMode { get; }

        bool ShowHelpAndExit { get; }
        
        bool ShowVersionAndExit { get; }
        
        IEnumerable<ICodeSource> CodeSources { get; }
        
        IEnumerable<string> NuGetSources { get; }
    }
}