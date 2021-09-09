// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class CSharpScriptRunner : ICSharpScriptRunner
    {
        private readonly ILog<CSharpScriptRunner> _log;
        private readonly IPresenter<ScriptState<object>> _scriptStatePresenter;
        private readonly IPresenter<CompilationDiagnostics> _diagnosticsPresenter;
        private ScriptState<object>? _scriptState;
        internal static readonly ScriptOptions Options = ScriptOptions.Default
            .AddImports("System")
            .WithLanguageVersion(LanguageVersion.Latest)
            .WithOptimizationLevel(OptimizationLevel.Release);

        public CSharpScriptRunner(
            ILog<CSharpScriptRunner> log,
            IPresenter<ScriptState<object>> scriptStatePresenter,
            IPresenter<CompilationDiagnostics> diagnosticsPresenter)
        {
            _log = log;
            _scriptStatePresenter = scriptStatePresenter;
            _diagnosticsPresenter = diagnosticsPresenter;
        }

        public bool Run(ICommand sourceCommand, string script)
        {
            var success = true;
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                _scriptState =
                    (_scriptState ?? CSharpScript.RunAsync(string.Empty, Options).Result)
                    .ContinueWithAsync(
                        script,
                        Options,
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