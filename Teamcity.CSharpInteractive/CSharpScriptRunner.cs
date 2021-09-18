// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics;
    using System.Linq;
    using Contracts;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class CSharpScriptRunner : ICSharpScriptRunner
    {
        private readonly ILog<CSharpScriptRunner> _log;
        private readonly IPresenter<ScriptState<object>> _scriptStatePresenter;
        private readonly IPresenter<CompilationDiagnostics> _diagnosticsPresenter;
        private readonly IScriptOptionsFactory _scriptOptionsFactory;
        private readonly IHost _host;
        private ScriptState<object>? _scriptState;
        
        public CSharpScriptRunner(
            ILog<CSharpScriptRunner> log,
            IPresenter<ScriptState<object>> scriptStatePresenter,
            IPresenter<CompilationDiagnostics> diagnosticsPresenter,
            IScriptOptionsFactory scriptOptionsFactory,
            IHost host)
        {
            _log = log;
            _scriptStatePresenter = scriptStatePresenter;
            _diagnosticsPresenter = diagnosticsPresenter;
            _scriptOptionsFactory = scriptOptionsFactory;
            _host = host;
        }

        public bool Run(ICommand sourceCommand, string script)
        {
            var success = true;
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                _scriptState =
                    (_scriptState ?? CSharpScript.RunAsync(string.Empty, _scriptOptionsFactory.Create(), _host, typeof(IHost)).Result)
                    .ContinueWithAsync(
                        script,
                        _scriptOptionsFactory.Create(),
                        exception =>
                        {
                            success = false;
                            _log.Error(ErrorId.Exception, new[] {new Text(exception.ToString())});
                            return true;
                        })
                    .Result;

                stopwatch.Stop();
                _log.Trace(new Text($"Time Elapsed {stopwatch.Elapsed:g}"));
                _diagnosticsPresenter.Show(new CompilationDiagnostics(sourceCommand, _scriptState.Script.GetCompilation().GetDiagnostics().ToList().AsReadOnly()));
            }
            catch (CompilationErrorException e)
            {
                _diagnosticsPresenter.Show(new CompilationDiagnostics(sourceCommand, e.Diagnostics.ToList().AsReadOnly()));
                success = false;
            }
            finally
            {
                if (_scriptState != null)
                {
                    _scriptStatePresenter.Show(_scriptState);
                }                
            }

            return success;
        }

        public void Reset()
        {
            _log.Trace("Reset state.");
            _scriptState = default;
        }
    }
}