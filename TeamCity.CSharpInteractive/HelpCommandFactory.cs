// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Text.RegularExpressions;

internal class HelpCommandFactory: ICommandFactory<string>
{
    private static readonly Regex Regex = new(@"^#help\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
    private readonly ILog<HelpCommandFactory> _log;

    public HelpCommandFactory(ILog<HelpCommandFactory> log) => _log = log;
        
    public int Order => 0;

    public IEnumerable<ICommand> Create(string replCommand)
    {
        if (!Regex.Match(replCommand).Success)
        {
            yield break;
        }

        _log.Trace(() => new []{new Text("REPL help")});
        yield return HelpCommand.Shared;
    }
}