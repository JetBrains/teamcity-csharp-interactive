// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    internal class Info : IInfo
    {
        private readonly IStdOut _stdOut;
        private readonly ISettings _settings;
        private readonly IPresenter<IEnumerable<ITraceSource>> _tracePresenter;
        private readonly IEnumerable<ITraceSource> _traceSources;

        public Info(
            IStdOut stdOut,
            ISettings settings,
            IPresenter<IEnumerable<ITraceSource>> tracePresenter,
            IEnumerable<ITraceSource> traceSources)
        {
            _stdOut = stdOut;
            _settings = settings;
            _tracePresenter = tracePresenter;
            _traceSources = traceSources;
        }
        
        public void ShowHeader()
        {
            _stdOut.Write(new Text(_settings.Title, Color.Header), Text.NewLine);
            if (_settings.InteractionMode == InteractionMode.Interactive)
            {
                _stdOut.Write(
                    new Text("Ctrl-C - Exit the REPL.", Color.Details), Text.NewLine,
                    new Text("#help  - Display help on available commands and key bindings.", Color.Details), Text.NewLine);
            }
        }

        public void ShowReplHelp() =>
            _stdOut.Write(
                new Text("Keyboard shortcuts:", Color.Header), Text.NewLine,
                new Text("    Enter        ", Color.Header), new Text("If the current submission appears to be complete, evaluate it. Otherwise, insert a new line."), Text.NewLine,
                new Text("    Escape       ", Color.Header), new Text("Clear the current submission."), Text.NewLine,
                new Text("    UpArrow      ", Color.Header), new Text("Replace the current submission with a previous submission."), Text.NewLine,
                new Text("    DownArrow    ", Color.Header), new Text("Replace the current submission with a subsequent submission (after having previously navigated backwards)."), Text.NewLine,
                new Text("    Ctrl-C       ", Color.Header), new Text("Exit the REPL."), Text.NewLine,
                new Text("REPL commands:", Color.Header), Text.NewLine,
                new Text("    #help        ", Color.Header), new Text("Display help on available commands and key bindings."), Text.NewLine,
                new Text("    #l           ", Color.Header), new Text($"Set verbosity level {string.Join(", ", Enum.GetValues(typeof(VerbosityLevel)).OfType<VerbosityLevel>().Select(i => i.ToString()))}, e.g. "), new Text("#l trace", Color.Details), new Text("."), Text.NewLine,
                new Text("Script directives:", Color.Header), Text.NewLine,
                new Text("    #r           ", Color.Header), new Text("Add a reference to a NuGet package or specified assembly and all its dependencies, e.g., "), new Text("#r MyPackage 1.2.3", Color.Details), new Text(" or "), new Text("#r \"myLib.dll\"", Color.Details), new Text("."), Text.NewLine,
                new Text("    #load        ", Color.Header), new Text("Load specified script file and execute it, e.g. "), new Text("#load \"myScript.csx\"", Color.Details), new Text("."), Text.NewLine
            );

        public void ShowFooter() => _tracePresenter.Show(_traceSources);
    }
}