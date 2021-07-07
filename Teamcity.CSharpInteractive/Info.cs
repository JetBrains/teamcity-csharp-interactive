// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Pure.DI;

    [ExcludeFromCodeCoverage]
    internal class Info : IInfo
    {
        private readonly Version _version;
        private readonly IStdOut _stdOut;
        private readonly ISettings _settings;
        private readonly IPresenter<IEnumerable<ITraceSource>> _tracePresenter;
        private readonly IEnumerable<ITraceSource> _traceSources;
        private readonly IDotnetEnvironment _dotnetEnvironment;

        public Info(
            [Tag("ToolVersion")] Version version,
            IStdOut stdOut,
            ISettings settings,
            IPresenter<IEnumerable<ITraceSource>> tracePresenter,
            IEnumerable<ITraceSource> traceSources,
            IDotnetEnvironment dotnetEnvironment)
        {
            _version = version;
            _stdOut = stdOut;
            _settings = settings;
            _tracePresenter = tracePresenter;
            _traceSources = traceSources;
            _dotnetEnvironment = dotnetEnvironment;
        }
        
        public void ShowHeader()
        {
            _stdOut.WriteLine(new Text("C# Interactive", Color.Header), new Text($" {_version} {_dotnetEnvironment.Tfm}", Color.Trace));
            if (_settings.InteractionMode == InteractionMode.Interactive)
            {
                _stdOut.WriteLine(new Text("Ctrl-C - Exit the REPL.", Color.Details));
                _stdOut.WriteLine(new Text("#help  - Display help on available commands and key bindings.", Color.Details));
            }
        }

        public void ShowReplHelp() =>
            _stdOut.WriteLine(
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
                new Text("    #load        ", Color.Header), new Text("Load specified script file and execute it, e.g. "), new Text("#load \"myScript.csx\"", Color.Details), new Text(".")
            );
        
        public void ShowHelp() =>
            _stdOut.WriteLine(
                Text.NewLine,
                new Text("Usage: dotnet csi [option] ... [script-file.csx] [script-argument] ..."), Text.NewLine,
                new Text("    script arguments are accessible in scripts via a global array called Args"), Text.NewLine,
                Text.NewLine,
                new Text("Executes script-file.csx if specified, otherwise launches an interactive REPL (Read Eval Print Loop)."), Text.NewLine,
                Text.NewLine,
                new Text("Options:"), Text.NewLine,
                new Text("    --help                           ", Color.Header), new Text("Display this usage message (alternative forms: /? -h /h /help)."), Text.NewLine,
                new Text("    --version                        ", Color.Header), new Text("Display the version and exit (alternative form: /version)."), Text.NewLine,
                new Text("    --source <NuGet package source>  ", Color.Header), new Text("NuGet package source (URL, UNC/folder path) to use (alternative forms: -s /source /s)."), Text.NewLine,
                new Text("    --property <key=value>           ", Color.Header), new Text("Define a key=value pair for the global dictionary called Props accessible in scripts (alternative forms: -p /property /p)."), Text.NewLine,
                new Text("    @<file>                          ", Color.Header), new Text("Read response file for more options."), Text.NewLine,
                new Text("    --                               ", Color.Header), new Text("Indicates that the remaining arguments should not be treated as options."), Text.NewLine
            );

        public void ShowVersion() =>
            _stdOut.WriteLine(new Text(_version.ToString()));

        public void ShowFooter() => _tracePresenter.Show(_traceSources);
    }
}