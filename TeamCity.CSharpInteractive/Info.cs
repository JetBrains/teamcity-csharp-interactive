// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Script;

[ExcludeFromCodeCoverage]
internal class Info : IInfo
{
    private readonly string _description;
    private readonly string _version;
    private readonly IStdOut _stdOut;
    private readonly ISettings _settings;
    private readonly IPresenter<IEnumerable<ITraceSource>> _tracePresenter;
    private readonly IEnumerable<ITraceSource> _traceSources;
    private readonly IEnumerable<ISettingDescription> _settingDescriptions;

    public Info(
        Assembly? assembly,
        IStdOut stdOut,
        ISettings settings,
        IPresenter<IEnumerable<ITraceSource>> tracePresenter,
        IEnumerable<ITraceSource> traceSources,
        IEnumerable<ISettingDescription> settingDescriptions)
    {
        _description = assembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? string.Empty;
        _version = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;
        _stdOut = stdOut;
        _settings = settings;
        _tracePresenter = tracePresenter;
        _traceSources = traceSources;
        _settingDescriptions = settingDescriptions;
    }
        
    public void ShowHeader()
    {
        _stdOut.WriteLine(new Text(_description, Color.Header));
        if (_settings.InteractionMode != InteractionMode.Interactive)
        {
            return;
        }

        _stdOut.WriteLine(new Text("Ctrl-C - Exit the REPL.", Color.Highlighted));
        _stdOut.WriteLine(new Text("#help  - Display help on available commands and key bindings.", Color.Highlighted));
    }

    public void ShowReplHelp()
    {
        var lines = new List<Text>
        {
            new("Keyboard shortcuts:", Color.Header), Text.NewLine,
            Text.Tab, new("Enter        ", Color.Header), new("If the current submission appears to be complete, evaluate it. Otherwise, insert a new line."), Text.NewLine,
            Text.Tab, new("Escape       ", Color.Header), new("Clear the current submission."), Text.NewLine,
            Text.Tab, new("UpArrow      ", Color.Header), new("Replace the current submission with a previous submission."), Text.NewLine,
            Text.Tab, new("DownArrow    ", Color.Header), new("Replace the current submission with a subsequent submission (after having previously navigated backwards)."), Text.NewLine,
            Text.Tab, new("Ctrl-C       ", Color.Header), new("Exit the REPL."), Text.NewLine,
            new("REPL commands:", Color.Header), Text.NewLine,
            Text.Tab, new("#help        ", Color.Header), new("Display help on available commands and key bindings."), Text.NewLine
        };

        var settingLines = 
            from setting in _settingDescriptions
            where setting.IsVisible
            select new []
            {
                Text.Tab, new Text($"#{setting.Key.PadRight(12, ' ')}", Color.Header),
                new Text($"{setting.Description} {string.Join(", ", Enum.GetValues(setting.SettingType).OfType<Enum>().Select(i => i.ToString()))}, e.g. "),
                new Text($"#{setting.Key} {Enum.GetValues(setting.SettingType).OfType<Enum>().LastOrDefault()}", Color.Highlighted),
                new Text("."),
                Text.NewLine
            };

        lines.AddRange(settingLines.SelectMany(i => i));
            
        lines.AddRange(new []
        {
            new Text("Script directives:", Color.Header), Text.NewLine,
            Text.Tab, new Text("#r           ", Color.Header), new Text("Add a reference to a NuGet package or specified assembly and all its dependencies, e.g., "), new Text("#r \"nuget:MyPackage, 1.2.3\"", Color.Highlighted), new Text(" or "), new Text("#r \"nuget:MyPackage\"", Color.Highlighted), new Text(" or "), new Text("#r \"MyLib.dll\"", Color.Highlighted), new Text("."), Text.NewLine,
            Text.Tab, new Text("#load        ", Color.Header), new Text("Load specified script file and execute it, e.g. "), new Text("#load \"script-file.csx\"", Color.Highlighted), new Text(".")
        });

        _stdOut.WriteLine(lines.ToArray());
    }

    public void ShowHelp() =>
        _stdOut.WriteLine(
            Text.NewLine,
            new Text("Usage: dotnet csi [options] [script-file.csx] [script-arguments]"), Text.NewLine,
            Text.Tab, new Text("script arguments are accessible in scripts via a global list called "), new Text("Args", Color.Highlighted), Text.NewLine,
            Text.NewLine,
            new Text("Executes \"script-file.csx\" if specified, otherwise launches an interactive REPL (Read Eval Print Loop)."), Text.NewLine,
            Text.NewLine,
            new Text("Options:"), Text.NewLine,
            Text.Tab, new Text("--help                           ", Color.Header), new Text("Display this usage message (alternative forms: /? -h /h /help)."), Text.NewLine,
            Text.Tab, new Text("--version                        ", Color.Header), new Text("Display the version and exit (alternative form: /version)."), Text.NewLine,
            Text.Tab, new Text("--source <NuGet package source>  ", Color.Header), new Text("NuGet package source (URL, UNC/folder path) to use (alternative forms: -s /source /s)."), Text.NewLine,
            Text.Tab, new Text("--property <key=value>           ", Color.Header), new Text("Define a key=value pair for the global dictionary called "), new Text("Props", Color.Highlighted), new Text(" accessible in scripts (alternative forms: -p /property /p)."), Text.NewLine,
            Text.Tab, new Text("@<file>                          ", Color.Header), new Text("Read response file for more options."), Text.NewLine,
            Text.Tab, new Text("--                               ", Color.Header), new Text("Indicates that the remaining arguments should not be treated as options."), Text.NewLine
        );

    public void ShowVersion() => _stdOut.WriteLine(new Text(_version));

    public void ShowFooter() => _tracePresenter.Show(_traceSources);
}