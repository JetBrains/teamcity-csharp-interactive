// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Text.RegularExpressions;

    internal class SetVerbosityLevelCommandParser: IReplCommandParser
    {
        private static readonly Regex Regex = new Regex(@"^l\s+(.+)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<SetVerbosityLevelCommandParser> _log;
        private readonly IStringService _stringService;

        public SetVerbosityLevelCommandParser(
            ILog<SetVerbosityLevelCommandParser> log,
            IStringService stringService)
        {
            _log = log;
            _stringService = stringService;
        }

        public bool TryParse(string replCommand, out ICommand? command)
        {
            var loadMatch = Regex.Match(replCommand);
            if (loadMatch.Success)
            {
                var rawParam = loadMatch.Groups[1].Value;
                Enum.TryParse<VerbosityLevel>(_stringService.TrimAndUnquote(rawParam), true, out var verbosityLevel);
                _log.Trace(new []{new Text($"REPL l {rawParam} -> {verbosityLevel}")});
                command = new SetVerbosityLevelCommand(verbosityLevel);
                return true;
            }

            command = default;
            return false;
        }
    }
}