// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    [ExcludeFromCodeCoverage]
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

        public bool? TryRun(ICommand command)
        {
            if (command.Kind != CommandKind.Script || command is not ScriptCommand scriptCommand)
            {
                return null;
            }

            var success = true;
            try
            {
                Script<object>? script = _scriptState == null 
                    ? CSharpScript.Create(scriptCommand.Script, Options)
                    : _scriptState.Script.ContinueWith(scriptCommand.Script);

                var compilation = script.GetCompilation();
                var diagnostics = compilation.GetDiagnostics();
                _diagnosticsPresenter.Show(diagnostics);
                if (!diagnostics.All(i => i.Severity != DiagnosticSeverity.Error))
                {
                    return false;
                }

                _scriptState = script.RunAsync(
                    null,
                    exception =>
                    {
                        success = false;
                        _log.Error(new[] {new Text(exception.ToString())});
                        return true;
                    }).Result;
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
            
            return success;
        }
    }
}