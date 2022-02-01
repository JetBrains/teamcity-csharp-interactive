// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class DiagnosticsPresenter : IPresenter<CompilationDiagnostics>
{
    private readonly ILog<DiagnosticsPresenter> _log;
    private readonly IErrorContext _errorContext;

    public DiagnosticsPresenter(ILog<DiagnosticsPresenter> log, IErrorContext errorContext)
    {
        _log = log;
        _errorContext = errorContext;
    }

    public void Show(CompilationDiagnostics data)
    {
        var (sourceCommand, readOnlyCollection) = data;
        var prefix = Text.Empty;
        if(_errorContext.TryGetSourceName(out var name))
        {
            prefix = new Text(name);
        }

        foreach (var diagnostic in readOnlyCollection)
        {
            switch (diagnostic.Severity)
            {
                case DiagnosticSeverity.Hidden:
                    _log.Trace(() => new[] {prefix, new Text(diagnostic.ToString())});
                    break;

                case DiagnosticSeverity.Info:
                    _log.Info(new[] {prefix, new Text(diagnostic.ToString())});
                    break;

                case DiagnosticSeverity.Warning:
                    _log.Warning(new[] {prefix, new Text(diagnostic.ToString())});
                    break;

                case DiagnosticSeverity.Error:
                    var errorId = $"{GetProperty(diagnostic.Id, string.Empty)},{diagnostic.Location.SourceSpan.Start},{diagnostic.Location.SourceSpan.Length}{GetProperty(GetFileName(sourceCommand.Name))}";
                    _log.Error(new ErrorId(errorId), new[] {prefix, new Text(diagnostic.ToString())});
                    break;
            }
        }
    }

    private static string GetProperty(string? value, string prefix = ",") =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : $"{prefix}{value}";

    private static string GetFileName(string file)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                return Path.GetFileName(file);
            }
        }
        catch
        {
            // ignored
        }

        return string.Empty;
    }
}