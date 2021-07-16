// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class SetVerbosityLevelCommandFactory: ICommandFactory<string>
    {
        private static readonly Regex Regex = new(@"^#l\s+(.+)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<SetVerbosityLevelCommandFactory> _log;
        private readonly IStringService _stringService;

        public SetVerbosityLevelCommandFactory(
            ILog<SetVerbosityLevelCommandFactory> log,
            IStringService stringService)
        {
            _log = log;
            _stringService = stringService;
        }
        
        public int Order => 0;

        public IEnumerable<ICommand> Create(string replCommand)
        {
            var loadMatch = Regex.Match(replCommand);
            if (!loadMatch.Success)
            {
                yield break;
            }

            var rawParam = loadMatch.Groups[1].Value;
            Enum.TryParse<VerbosityLevel>(_stringService.TrimAndUnquote(rawParam), true, out var verbosityLevel);
            _log.Trace(new []{new Text($"REPL l {rawParam} -> {verbosityLevel}")});
            yield return new SetVerbosityLevelCommand(verbosityLevel);
        }
    }
}