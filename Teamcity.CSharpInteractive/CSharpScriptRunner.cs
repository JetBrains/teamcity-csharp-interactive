// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class CSharpScriptRunner : ICSharpScriptRunner
    {
        private readonly ILog<CSharpScriptRunner> _log;
        private readonly IPresenter<ScriptState<object>> _scriptStatePresenter;
        private readonly IPresenter<IEnumerable<Diagnostic>> _diagnosticsPresenter;
        private ScriptState<object>? _scriptState;
        private static readonly ScriptOptions Options = ScriptOptions.Default
            .AddImports("System");

        public CSharpScriptRunner(
            ILog<CSharpScriptRunner> log,
            IPresenter<ScriptState<object>> scriptStatePresenter,
            IPresenter<IEnumerable<Diagnostic>> diagnosticsPresenter)
        {
            _log = log;
            _scriptStatePresenter = scriptStatePresenter;
            _diagnosticsPresenter = diagnosticsPresenter;
        }

        public bool Run(string script)
        {
            var success = true;
            try
            {
                _scriptState = 
                    (_scriptState ?? CSharpScript.RunAsync(string.Empty, Options).Result)
                    .ContinueWithAsync(
                        script, 
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

            return success;
        }

        public void Reset() => _scriptState = default;
    }
}