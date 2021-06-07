// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class Info : IInfo
    {
        private readonly ILog<Info> _log;
        private readonly ISettings _settings;
        private readonly IPresenter<IEnumerable<ITraceSource>> _tracePresenter;
        private readonly IEnumerable<ITraceSource> _traceSources;

        public Info(
            ILog<Info> log,
            ISettings settings,
            IPresenter<IEnumerable<ITraceSource>> tracePresenter,
            IEnumerable<ITraceSource> traceSources)
        {
            _log = log;
            _settings = settings;
            _tracePresenter = tracePresenter;
            _traceSources = traceSources;
        }
        
        public void ShowHeader()
        {
            _log.Info(new Text(_settings.Title, Color.Header));
        }

        public void ShowReplHelp()
        {
            _log.Info(
                new Text("Keyboard shortcuts:", Color.Header), Text.NewLine,
                new Text("    Enter        ", Color.Header), new Text("If the current submission appears to be complete, evaluate it. Otherwise, insert a new line."), Text.NewLine,
                new Text("    Escape       ", Color.Header), new Text("Clear the current submission."), Text.NewLine,
                new Text("    UpArrow      ", Color.Header), new Text("Replace the current submission with a previous submission."), Text.NewLine,
                new Text("    DownArrow    ", Color.Header), new Text("Replace the current submission with a subsequent submission (after having previously navigated backwards)."), Text.NewLine,
                new Text("    Ctrl-C       ", Color.Header), new Text("Exit the REPL."), Text.NewLine,
                new Text("REPL commands:", Color.Header), Text.NewLine,
                new Text("    #help        ", Color.Header), new Text("Display help on available commands and key bindings."), Text.NewLine,
                new Text("    #l           ", Color.Header), new Text("Set logging verbosity level normal or trace, e.g. #l trace."), Text.NewLine,
                new Text("Script directives:", Color.Header), Text.NewLine,
                new Text("    #r           ", Color.Header), new Text("Add a metadata reference to specified assembly and all its dependencies, e.g. #r \"myLib.dll\"."), Text.NewLine,
                new Text("    #load        ", Color.Header), new Text("Load specified script file and execute it, e.g. #load \"myScript.csx\".")
            );
        }

        public void ShowFooter() => _tracePresenter.Show(_traceSources);
    }
}