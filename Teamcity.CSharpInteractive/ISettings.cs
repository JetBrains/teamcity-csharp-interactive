namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ISettings
    {
        VerbosityLevel VerbosityLevel { get; set; }

        InteractionMode InteractionMode { get; }

        bool ShowHelpAndExit { get; }
        
        bool ShowVersionAndExit { get; }
        
        IEnumerable<ICodeSource> CodeSources { get; }
        
        IEnumerable<string> ScriptArguments { get; }
        
        IEnumerable<string> NuGetSources { get; }
    }
}