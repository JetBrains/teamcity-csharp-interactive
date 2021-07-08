// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Threading;
    using Host;

    internal class CSharpScriptCommandRunner : ICommandRunner
    {
        private readonly ILog<CSharpScriptCommandRunner> _log;
        private readonly ICSharpScriptRunner _scriptRunner;
        private readonly IObservable<SessionContent> _sessionsSource;

        public CSharpScriptCommandRunner(
            ILog<CSharpScriptCommandRunner> log,
            ICSharpScriptRunner scriptRunner,
            IObservable<SessionContent> sessionsSource)
        {
            _log = log;
            _scriptRunner = scriptRunner;
            _sessionsSource = sessionsSource;
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
                        var sessionObserver = new SessionObserver(finisEvent);
                        using (_sessionsSource.Subscribe(sessionObserver))
                        {
                            _scriptRunner.Run($"{nameof(Host.ScriptInternal_FinishCommand)}();");
                            if (!finisEvent.WaitOne(5000))
                            {
                                _log.Trace("Timeout while waiting for a finish of a command.");
                            }
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
        
        private class SessionObserver: IObserver<SessionContent>
        {
            private readonly ManualResetEvent _resetEvent;

            public SessionObserver(ManualResetEvent resetEvent) => _resetEvent = resetEvent;

            public void OnNext(SessionContent value) => _resetEvent.Set();

            public void OnError(Exception error) { }

            public void OnCompleted() { }
        }
    }
}