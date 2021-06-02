// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class CommandSource : ICommandSource
    {
        private readonly ILog<CommandSource> _log;
        private readonly ISettings _settings;
        private readonly IReplCommandFactory[] _replCommandFactories;
        private readonly IScriptCommandFactory _scriptCommandFactory;

        public CommandSource(
            ILog<CommandSource> log,
            ISettings settings,
            IReplCommandFactory[] replCommandFactories,
            IScriptCommandFactory scriptCommandFactory)
        {
            _log = log;
            _settings = settings;
            _replCommandFactories = replCommandFactories;
            _scriptCommandFactory = scriptCommandFactory;
        }
        
        public IEnumerable<ICommand> GetCommands() => _settings.Sources.SelectMany(GetCommands);

        private IEnumerable<ICommand> GetCommands(ICodeSource codeSource)
        {
            using var sourceBlockToken = _log.Block(codeSource.Name);
            foreach (var code in codeSource)
            {
                var sb = new StringBuilder();
                foreach (var line in code.Split(System.Environment.NewLine))
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("#"))
                    {
                        var hasReplCommand = false;
                        foreach (var parser in _replCommandFactories)
                        {
                            var commands = parser.TryCreate(trimmedLine).ToArray();
                            if (!commands.Any())
                            {
                                continue;
                            }

                            if (sb.Length > 0)
                            {
                                yield return _scriptCommandFactory.Create(codeSource.Name, sb.ToString());
                                sb.Clear();
                            }

                            hasReplCommand = true;
                            foreach (var command in commands)
                            {
                                yield return command;
                            }

                            break;
                        }

                        if (!hasReplCommand)
                        {
                            sb.AppendLine(line);
                        }
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }

                if (sb.Length > 0)
                {
                    yield return _scriptCommandFactory.Create(codeSource.Name, sb.ToString());
                }
            }
        }
    }
}