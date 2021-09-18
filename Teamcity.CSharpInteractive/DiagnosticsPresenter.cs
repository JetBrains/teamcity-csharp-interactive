// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Microsoft.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class DiagnosticsPresenter: IPresenter<CompilationDiagnostics>
    {
        private readonly ILog<DiagnosticsPresenter> _log;

        public DiagnosticsPresenter(ILog<DiagnosticsPresenter> log) => _log = log;

        public void Show(CompilationDiagnostics data)
        {
            foreach (var diagnostic in data.Diagnostics)
            {
                switch (diagnostic.Severity)
                {
                    case DiagnosticSeverity.Hidden:
                        _log.Trace(new []{new Text(diagnostic.ToString())});
                        break;

                    case DiagnosticSeverity.Info:
                        _log.Info(new []{new Text(diagnostic.ToString())});
                        break;

                    case DiagnosticSeverity.Warning:
                        _log.Warning(new []{new Text(diagnostic.ToString())});
                        break;

                    case DiagnosticSeverity.Error:
                        var errorId = $"{GetProperty(diagnostic.Id, string.Empty)},{diagnostic.Location.SourceSpan.Start},{diagnostic.Location.SourceSpan.Length}{GetProperty(GetFileName(data.SourceCommand.Name))}";
                        _log.Error(new ErrorId(errorId), new []{new Text(diagnostic.ToString())});
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
}