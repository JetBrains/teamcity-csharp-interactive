// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System.Threading;
    using Host;

    internal class CSharpScriptCommandRunner : ICommandRunner
    {
        private readonly ILog<CSharpScriptCommandRunner> _log;
        private readonly ICSharpScriptRunner _scriptRunner;
        private readonly IFlow _flow;

        public CSharpScriptCommandRunner(
            ILog<CSharpScriptCommandRunner> log,
            ICSharpScriptRunner scriptRunner,
            IFlow flow)
        {
            _log = log;
            _scriptRunner = scriptRunner;
            _flow = flow;
        }

        public CommandResult TryRun(ICommand command)
        {
            switch (command)
            {
                case ScriptCommand scriptCommand:
                    var result = new CommandResult(command, _scriptRunner.Run(scriptCommand.Script));
                    if (!command.Internal)
                    {
                        using var finisEvent = new ManualResetEvent(false);
                        void FlowOnOnCompleted()  {  finisEvent.Set(); }
                        _flow.OnCompleted += FlowOnOnCompleted;
                        try
                        {
                            _scriptRunner.Run($"{nameof(Host.ScriptInternal_FinishCommand)}();");
                            if (!finisEvent.WaitOne(10000))
                            {
                                _log.Trace("Timeout while waiting for a finish of a command.");
                            }
                        }
                        finally
                        {
                            _flow.OnCompleted -= FlowOnOnCompleted;
                        }
                    }

                    return result;

                case ResetCommand:
                    _scriptRunner.Reset();
                    return new CommandResult(command, true);

                default:
                    return new CommandResult(command, default);
            }
        }
    }
}