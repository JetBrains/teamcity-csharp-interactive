// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HostApi;
using Microsoft.CodeAnalysis.CSharp;

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
        LanguageVersion languageVersion,
        IStdOut stdOut,
        ISettings settings,
        IPresenter<IEnumerable<ITraceSource>> tracePresenter,
        IEnumerable<ITraceSource> traceSources,
        IEnumerable<ISettingDescription> settingDescriptions)
    {
        _description = string.Format(assembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "C# {0} script runner", languageVersion.ToDisplayString());
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
            select new[]
            {
                Text.Tab, new Text($"#{setting.Key,-12}", Color.Header),
                new Text($"{setting.Description} {string.Join(", ", Enum.GetValues(setting.SettingType).OfType<Enum>().Select(i => i.ToString()))}, e.g. "),
                new Text($"#{setting.Key} {Enum.GetValues(setting.SettingType).OfType<Enum>().LastOrDefault()}", Color.Highlighted),
                new Text("."),
                Text.NewLine
            };

        lines.AddRange(settingLines.SelectMany(i => i));

        lines.AddRange(new[]
        {
            new Text("Script directives:", Color.Header), Text.NewLine,
            Text.Tab, new Text("#r           ", Color.Header), new Text("Add a reference to a NuGet package or specified assembly and all its dependencies, e.g., "), new Text("#r \"nuget:MyPackage, 1.2.3\"", Color.Highlighted), new Text(" or "), new Text("#r \"nuget:MyPackage\"", Color.Highlighted), new Text(" or "), new Text("#r \"MyLib.dll\"", Color.Highlighted), new Text("."), Text.NewLine,
            Text.Tab, new Text("#load        ", Color.Header), new Text("Load specified script file and execute it, e.g. "), new Text("#load \"script-file.csx\"", Color.Highlighted), new Text(".")
        });

        _stdOut.WriteLine(lines.ToArray());
    }

    public void ShowHelp() =>
        _stdOut.WriteLine(
            new []
            {
                new[]
                {
                    Text.NewLine,
                    new("Usage:", Color.Header),
                    new(" dotnet csi [options] [--] [script] [script arguments]"), Text.NewLine,
                    Text.NewLine,
                    new("Executes a script if specified, otherwise launches an interactive REPL (Read Eval Print Loop)."),
                    Text.NewLine,
                    Text.NewLine
                },
                Option(
                    "script",
                    new Text("The path to the script file to run."),
                    Text.NewLine, Text.Tab,
                    new Text("If no such file is found, the command will treat it as a directory"),
                    Text.NewLine, Text.Tab,
                    new Text("and look for a single script file inside that directory.")),
                Option(
                    "script arguments",
                    new Text("Script arguments are accessible in a script via the global list "),
                    new Text("Args[index]", Color.Details),
                    new Text(" by an argument index.")),
                Option("--", new Text("Indicates that the remaining arguments should not be treated as options.")),
                Option(
                    "--help",
                    new Text("Display this usage message (alternative forms: /? -h /h /help).")),
                Option("--version", new Text("Display the version and exit (alternative form: /version).")),
                Option("--source <NuGet package source>", new Text("NuGet package source (URL, UNC/folder path) to use (alternative forms: -s /source /s),"),
                    Text.NewLine, Text.Tab,
                    new Text("for example: "),
                    new Text("--source https://api.nuget.org/v3/index.json", Color.Details)),
                Option("--property <key=value>[;<keyN=valueN>]", 
                    new Text("Define a key-value pair(s) for the script properties "),
                    new Text("(*1)", Color.Highlighted),
                    new Text(" accessible in scripts."),
                    Text.NewLine, Text.Tab,
                    new Text("Alternative forms:"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("-property"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("-p"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("/property"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("/p"),
                    Text.NewLine, Text.Tab,
                    new Text("Specify each property separately, or use a semicolon or comma to separate multiple properties,"),
                    Text.NewLine, Text.Tab,
                    new Text("as the following example shows: "),
                    new Text("--property key1=val1;key2=val2", Color.Details)),
                Option("--property:<key=value>[;<keyN=valueN>]",
                    new Text("Define a key-value pair(s) in MSBuild style for the script properties "),
                    new Text("(*1)", Color.Highlighted),
                    new Text(" accessible in scripts."),
                    Text.NewLine, Text.Tab,
                    new Text("Alternative forms:"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("-property:<key=value>"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("-p:<key=value>"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("/property:<key=value>"),
                    Text.NewLine, Text.Tab, Text.Tab,
                    new Text("/p:<key=value>"),
                    Text.NewLine, Text.Tab,
                    new Text("Specify each property separately, or use a semicolon or comma to separate multiple properties,"),
                    Text.NewLine, Text.Tab,
                    new Text("as the following example shows: "),
                    new Text("--property:key1=val1;key2=val2", Color.Details)),
                Option("@<file>", new Text("Read response file for more options as the following example shows: "),
                    new Text("@OptionsDir/MyOptions.rsp", Color.Details)),
                new[]
                {
                    new("Notes:", Color.Header),
                    Text.NewLine,
                    Text.Tab,
                    new("- "),
                    new("*1", Color.Highlighted),
                    new(" script properties are accessible a script via the global dictionary "),
                    new("Props[\"name\"]", Color.Details),
                    new(" by a property name"),
                    Text.NewLine,
                    Text.Tab,
                    new("- "),
                    new("using HostApi;", Color.Details),
                    new Text(" directive in a script allows you to use host API types"),
                    Text.NewLine,
                    Text.Tab,
                    new Text("  without specifying the fully qualified namespace of these types")
                }
            }.SelectMany(i => i).ToArray()
        );

    public void ShowVersion() => _stdOut.WriteLine(new Text(_version));

    public void ShowFooter() => _tracePresenter.Show(_traceSources);

    private static Text[] Option(string option, params Text[] description)
    {
        return new []
        {
            new []
            {
                new Text(option, Color.Header),
                Text.NewLine,
                Text.Tab
            },
            description,
            new []
            {
            Text.NewLine,
            Text.NewLine,
            }
        }.SelectMany(i => i).ToArray();
    }
}