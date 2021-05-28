// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Text.RegularExpressions;

    internal class HelpCommandParser: IReplCommandParser
    {
        private static readonly Regex Regex = new Regex(@"^help\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<HelpCommandParser> _log;

        public HelpCommandParser(ILog<HelpCommandParser> log) => _log = log;

        public bool TryParse(string replCommand, out ICommand? command)
        {
            if (Regex.Match(replCommand).Success)
            {
                _log.Trace(new []{new Text("REPL help")});
                command = HelpCommand.Shared;
                return true;
            }

            command = default;
            return false;
        }
    }
}