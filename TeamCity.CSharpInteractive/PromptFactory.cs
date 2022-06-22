namespace TeamCity.CSharpInteractive;

using PrettyPrompt;
using PrettyPrompt.Consoles;
using PrettyPrompt.Highlighting;
using Microsoft.CodeAnalysis.Completion;

internal class PromptFactory
{
    private readonly IEnvironment _environment;

    public PromptFactory(IEnvironment environment)
    {
        _environment = environment;
    }
    
    /*public IPrompt Create()
    {
        var commitCompletion = new KeyPressPatterns(
            Microsoft.CodeAnalysis.CompletionRules.Default.DefaultCommitCharacters
                .Except(new[] { ' ', '=' })
                .Select(i => new KeyPressPattern(i))
                .Concat(new KeyPressPattern[] { new(ConsoleKey.Enter), new(ConsoleKey.Tab) })
                .ToArray());
        
        return new Prompt(
            persistentHistoryFilepath: _environment.GetPath(SpecialFolder.Data),
            callbacks: new CSharpReplPromptCallbacks(console, roslyn, config),
            configuration: new PromptConfiguration(
                keyBindings: config.KeyBindings,
                prompt: new FormattedString(">"),
                completionBoxBorderFormat: config.Theme.GetCompletionBoxBorderFormat(),
                completionItemDescriptionPaneBackground: config.Theme.GetCompletionItemDescriptionPaneBackground(),
                selectedCompletionItemBackground: config.Theme.GetSelectedCompletionItemBackgroundColor(),
                selectedTextBackground: config.Theme.GetSelectedTextBackground(),
                tabSize: config.TabSize));
    }*/
}