// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal class CommandSource : ICommandSource
    {
        private readonly ILog<CommandSource> _log;
        private readonly ISettings _settings;
        private readonly IReplCommandParser[] _replCommandParsers;
        private readonly IScriptCommandParser _scriptCommandParser;

        public CommandSource(
            ILog<CommandSource> log,
            ISettings settings,
            IReplCommandParser[] replCommandParsers,
            IScriptCommandParser scriptCommandParser)
        {
            _log = log;
            _settings = settings;
            _replCommandParsers = replCommandParsers;
            _scriptCommandParser = scriptCommandParser;
        }

        public IEnumerable<ICommand> GetCommands()
        {
            foreach (var codeSource in _settings.CodeSources)
            {
                foreach (var code in codeSource)
                {
                    if (!_scriptCommandParser.HasCode)
                    {
                        var trimmedCode = code.Trim();
                        if (trimmedCode.StartsWith("#"))
                        {
                            var replCommand = trimmedCode.Substring(1, trimmedCode.Length - 1).Trim();
                            var parsed = false;
                            foreach (var parser in _replCommandParsers)
                            {
                                if (parser.TryParse(replCommand, out var command))
                                {
                                    parsed = true;
                                    yield return command!;
                                    break;
                                }
                            }

                            if (!parsed)
                            {
                                _log.Error(new []{new Text($"Unknown REPL command {replCommand}. Type #help for help.")});
                            }
                            
                            continue;
                        }
                    }
                    
                    yield return _scriptCommandParser.Parse(codeSource.Name, code);
                }
            }
        }
    }
}