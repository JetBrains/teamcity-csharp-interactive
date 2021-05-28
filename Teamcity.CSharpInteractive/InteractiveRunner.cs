// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    internal class InteractiveRunner : IRunner
    {
        private readonly ICommandSource _commandSource;
        private readonly ICommandRunner[] _commandRunners;
        private readonly IStdOut _stdOut;
        
        public InteractiveRunner(
            ICommandSource commandSource,
            ICommandRunner[] commandRunners,
            IStdOut stdOut)
        {
            _commandSource = commandSource;
            _commandRunners = commandRunners;
            _stdOut = stdOut;
        }

        public InteractionMode InteractionMode => InteractionMode.Interactive;

        public ExitCode Run()
        {
            _stdOut.Write(new Text("> "));
            foreach (var command in _commandSource.GetCommands())
            {
                foreach (var runner in _commandRunners)
                {
                    bool? result;
                    switch (command.Kind)
                    {
                        case CommandKind.Code:
                            result = true;
                            _stdOut.Write(new Text(". "));
                            break;
                    
                        default:
                            result = runner.TryRun(command);
                            if (result != null)
                            {
                                _stdOut.Write(new Text("> "));
                            }

                            break;
                    }

                    if (result.HasValue)
                    {
                        break;
                    }
                }
            }

            return ExitCode.Success;
        }
    }
}