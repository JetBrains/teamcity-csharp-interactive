// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HostApi;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

[ExcludeFromCodeCoverage]
internal class CSharpScriptRunner : ICSharpScriptRunner
{
    private readonly ILog<CSharpScriptRunner> _log;
    private readonly IPresenter<ScriptState<object>> _scriptStatePresenter;
    private readonly IPresenter<CompilationDiagnostics> _diagnosticsPresenter;
    private readonly IReadOnlyCollection<IScriptOptionsFactory> _scriptOptionsFactories;
    private readonly IExitCodeParser _exitCodeParser;
    private ScriptState<object>? _scriptState;

    public CSharpScriptRunner(
        ILog<CSharpScriptRunner> log,
        IPresenter<ScriptState<object>> scriptStatePresenter,
        IPresenter<CompilationDiagnostics> diagnosticsPresenter,
        IReadOnlyCollection<IScriptOptionsFactory> scriptOptionsFactories,
        IExitCodeParser exitCodeParser,
        IHost host)
    {
        _log = log;
        _scriptStatePresenter = scriptStatePresenter;
        _diagnosticsPresenter = diagnosticsPresenter;
        _scriptOptionsFactories = scriptOptionsFactories;
        _exitCodeParser = exitCodeParser;
    }

    public CommandResult Run(ICommand sourceCommand, string script)
    {
        var success = true;
        try
        {
            var options = _scriptOptionsFactories.Aggregate(ScriptOptions.Default, (current, scriptOptionsFactory) => scriptOptionsFactory.Create(current));
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _scriptState =
                (_scriptState ?? CSharpScript.RunAsync(string.Empty, options).Result)
                .ContinueWithAsync(
                    script,
                    options,
                    exception =>
                    {
                        success = false;
                        _log.Error(ErrorId.Exception, new[] {new Text(exception.ToString())});
                        return true;
                    })
                .Result;

            stopwatch.Stop();
            _log.Trace(() => new[] {new Text($"Time Elapsed {stopwatch.Elapsed:g}")});
            _diagnosticsPresenter.Show(new CompilationDiagnostics(sourceCommand, _scriptState.Script.GetCompilation().GetDiagnostics().ToList().AsReadOnly()));
            if (_scriptState.ReturnValue != default)
            {
                if (_exitCodeParser.TryParse(_scriptState.ReturnValue, out var exitCode))
                {
                    return new CommandResult(sourceCommand, success, exitCode);
                }

                _log.Trace(() => new[] {new Text("Return value is \""), new Text(_scriptState.ReturnValue.ToString() ?? "empty"), new Text("\".")});
                return new CommandResult(sourceCommand, success);
            }
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

        return new CommandResult(sourceCommand, success);
    }

    public void Reset()
    {
        _log.Trace(() => new[] {new Text("Reset state.")});
        _scriptState = default;
    }
}