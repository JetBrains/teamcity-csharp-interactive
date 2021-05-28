// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class DiagnosticsPresenter: IPresenter<IEnumerable<Diagnostic>>
    {
        private readonly ILog<DiagnosticsPresenter> _log;

        public DiagnosticsPresenter(ILog<DiagnosticsPresenter> log) => _log = log;

        public void Show(IEnumerable<Diagnostic> data)
        {
            foreach (var diagnostic in data)
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
                        _log.Error(new []{new Text(diagnostic.ToString())});
                        break;
                }
            }
        }
    }
}