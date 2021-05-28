// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Text.RegularExpressions;

    internal class LoadScriptCommandParser: IReplCommandParser
    {
        private static readonly Regex LoadRegex = new Regex(@"^load\s+(.+)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<LoadScriptCommandParser> _log;
        private readonly IStringService _stringService;

        public LoadScriptCommandParser(
            ILog<LoadScriptCommandParser> log,
            IStringService stringService)
        {
            _log = log;
            _stringService = stringService;
        }

        public bool TryParse(string replCommand, out ICommand? command)
        {
            var loadMatch = LoadRegex.Match(replCommand);
            if (loadMatch.Success)
            {
                var rawParam = loadMatch.Groups[1].Value;
                var param = _stringService.TrimAndUnquote(rawParam); 
                _log.Trace(new []{new Text($"REPL load {rawParam} -> {param}")});
                command = new LoadScriptCommand(param);
                return true;
            }

            command = default;
            return false;
        }
    }
}