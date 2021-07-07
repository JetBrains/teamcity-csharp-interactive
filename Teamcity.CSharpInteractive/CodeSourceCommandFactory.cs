// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class CodeSourceCommandFactory: ICommandFactory<ICodeSource>
    {
        private readonly ILog<CodeSourceCommandFactory> _log;
        private readonly ICommandFactory<string>[] _replCommandFactories;
        private readonly ICommandFactory<ScriptCommand> _scriptCommandFactory;

        public CodeSourceCommandFactory(
            ILog<CodeSourceCommandFactory> log,
            ICommandFactory<string>[] replCommandFactories,
            ICommandFactory<ScriptCommand> scriptCommandFactory)
        {
            _log = log;
            _replCommandFactories = replCommandFactories;
            _scriptCommandFactory = scriptCommandFactory;
        }

        public IEnumerable<ICommand> Create(ICodeSource codeSource)
        {
            using var sourceBlockToken = _log.Block(codeSource.Name);
            foreach (var code in codeSource)
            {
                var sb = new StringBuilder();
                foreach (var line in code.Split(System.Environment.NewLine))
                {
                    var trimmedLine = line.Trim();
                    _log.Trace($"Line: \"{trimmedLine}\".");
                    if (trimmedLine.StartsWith("#"))
                    {
                        var hasReplCommand = false;
                        foreach (var replCommandFactory in _replCommandFactories)
                        {
                            var commands = replCommandFactory.Create(trimmedLine).ToArray();
                            _log.Trace($"REPL commands count: {commands.Length}.");
                            if (!commands.Any())
                            {
                                continue;
                            }

                            if (sb.Length > 0)
                            {
                                _log.Trace($"Yield script commands before REPL commands.");
                                foreach (var command in _scriptCommandFactory.Create(new ScriptCommand(codeSource.Name, sb.ToString(), codeSource.Internal)))
                                {
                                    yield return command;
                                }

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
                    _log.Trace($"Finally yield script commands.");
                    foreach (var command in _scriptCommandFactory.Create(new ScriptCommand(codeSource.Name, sb.ToString(), codeSource.Internal)))
                    {
                        yield return command;
                    }
                }
            }
        }
    }
}