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
                    if (trimmedLine.StartsWith("#"))
                    {
                        var hasReplCommand = false;
                        foreach (var parser in _replCommandFactories)
                        {
                            var commands = parser.Create(trimmedLine).ToArray();
                            if (!commands.Any())
                            {
                                continue;
                            }

                            if (sb.Length > 0)
                            {
                                foreach (var command in _scriptCommandFactory.Create(new ScriptCommand(codeSource.Name, sb.ToString())))
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
                    foreach (var command in _scriptCommandFactory.Create(new ScriptCommand(codeSource.Name, sb.ToString())))
                    {
                        yield return command;
                    }
                }
            }
        }
    }
}