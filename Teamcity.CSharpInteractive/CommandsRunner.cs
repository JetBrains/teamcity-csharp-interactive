// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal class CommandsRunner : ICommandsRunner
    {
        private readonly ICommandRunner[] _commandRunners;
        private readonly IStatistics _statistics;

        public CommandsRunner(
            ICommandRunner[] commandRunners,
            IStatistics statistics)
        {
            _commandRunners = commandRunners;
            _statistics = statistics;
        }

        public IEnumerable<CommandResult> Run(IEnumerable<ICommand> commands)
        {
            using (_statistics.Start())
            {
                foreach (var command in commands)
                {
                    var processed = false;
                    foreach (var runner in _commandRunners)
                    {
                        var result = runner.TryRun(command);
                        if (result.Success.HasValue)
                        {
                            processed = true;
                            yield return result;
                            break;
                        }
                    }

                    if (!processed)
                    {
                        yield return new CommandResult(command, default);
                    }
                }
            }
        }
    }
}