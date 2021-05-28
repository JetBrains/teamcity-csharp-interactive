// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Text.RegularExpressions;

    internal class AddReferenceCommandParser: IReplCommandParser
    {
        private static readonly Regex Regex = new Regex(@"^r\s+(.+)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<AddReferenceCommandParser> _log;
        private readonly IStringService _stringService;

        public AddReferenceCommandParser(
            ILog<AddReferenceCommandParser> log,
            IStringService stringService)
        {
            _log = log;
            _stringService = stringService;
        }

        public bool TryParse(string replCommand, out ICommand? command)
        {
            var referenceMatch = Regex.Match(replCommand);
            if (referenceMatch.Success)
            {
                var rawParam = referenceMatch.Groups[1].Value;
                var param =_stringService. TrimAndUnquote(rawParam); 
                _log.Trace(new []{new Text($"REPL r {rawParam} -> {param}")});
                command =new AddReferenceCommand(param);
                return true;
            }

            command = default;
            return false;
        }
    }
}