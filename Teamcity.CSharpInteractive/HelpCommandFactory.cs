// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class HelpCommandFactory: IReplCommandFactory
    {
        private static readonly Regex Regex = new(@"^#help\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<HelpCommandFactory> _log;

        public HelpCommandFactory(ILog<HelpCommandFactory> log) => _log = log;

        public IEnumerable<ICommand> TryCreate(string replCommand)
        {
            if (!Regex.Match(replCommand).Success)
            {
                yield break;
            }

            _log.Trace(new []{new Text("REPL help")});
            yield return HelpCommand.Shared;
        }
    }
}