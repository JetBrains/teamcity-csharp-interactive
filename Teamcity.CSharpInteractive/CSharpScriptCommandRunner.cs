// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class CSharpScriptCommandRunner : ICommandRunner
    {
        private readonly ILog<CSharpScriptCommandRunner> _log;
        private readonly IPresenter<ScriptState<object>> _scriptStatePresenter;
        private readonly IPresenter<IEnumerable<Diagnostic>> _diagnosticsPresenter;
        private ScriptState<object>? _scriptState;

        private static readonly ScriptOptions Options = ScriptOptions.Default
            .AddImports("System");

        public CSharpScriptCommandRunner(
            ILog<CSharpScriptCommandRunner> log,
            IPresenter<ScriptState<object>> scriptStatePresenter,
            IPresenter<IEnumerable<Diagnostic>> diagnosticsPresenter)
        {
            _log = log;
            _scriptStatePresenter = scriptStatePresenter;
            _diagnosticsPresenter = diagnosticsPresenter;
        }

        public CommandResult TryRun(ICommand command)
        {
            if (command.Kind != CommandKind.Script || command is not ScriptCommand scriptCommand)
            {
                return new CommandResult(command, default);
            }
            
            var success = true;
            try
            {
                _scriptState = 
                    (_scriptState ?? CSharpScript.RunAsync(string.Empty, Options).Result)
                    .ContinueWithAsync(
                        scriptCommand.Script, 
                        Options,
                        exception =>
                        {
                            success = false;
                            _log.Error(new[] {new Text(exception.ToString())});
                            return true;
                        })
                    .Result;
                    
                _diagnosticsPresenter.Show(_scriptState.Script.GetCompilation().GetDiagnostics());
            }
            catch (CompilationErrorException e)
            {
                _diagnosticsPresenter.Show(e.Diagnostics);
                success = false;
            }

            if (_scriptState != null)
            {
                _scriptStatePresenter.Show(_scriptState);
            }
            
            return new CommandResult(command, success);
        }
    }
}