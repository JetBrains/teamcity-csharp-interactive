// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections.Immutable;

internal class Settings : ISettings, ISettingSetter<VerbosityLevel>
{
    private readonly object _lockObject = new();
    private readonly RunningMode _runningMode;
    private readonly IEnvironment _environment;
    private readonly ICommandLineParser _commandLineParser;
    private readonly ICodeSource _consoleCodeSource;
    private readonly Func<string, ICodeSource> _fileCodeSourceFactory;
    private bool _isLoaded;
    private VerbosityLevel _verbosityLevel = VerbosityLevel.Normal;
    private InteractionMode _interactionMode = InteractionMode.Interactive;
    private bool _showHelpAndExit;
    private bool _showVersionAndExit;
    private IEnumerable<ICodeSource> _codeSources = Enumerable.Empty<ICodeSource>();
    private IReadOnlyList<string> _scriptArguments = ImmutableArray<string>.Empty;
    private IReadOnlyDictionary<string, string> _scriptProperties = new Dictionary<string, string>();
    private IEnumerable<string> _nuGetSources = Enumerable.Empty<string>();

    public Settings(
        RunningMode runningMode,
        IEnvironment environment,
        ICommandLineParser commandLineParser,
        ICodeSource consoleCodeSource,
        [Tag(typeof(LoadFileCodeSource))] Func<string, ICodeSource> fileCodeSourceFactory)
    {
        _runningMode = runningMode;
        _environment = environment;
        _commandLineParser = commandLineParser;
        _consoleCodeSource = consoleCodeSource;
        _fileCodeSourceFactory = fileCodeSourceFactory;
    }

    public VerbosityLevel VerbosityLevel
    {
        get
        {
            EnsureLoaded();
            return _verbosityLevel;
        }
    }

    public InteractionMode InteractionMode
    {
        get
        {
            EnsureLoaded();
            return _interactionMode;
        }
    }

    public bool ShowHelpAndExit
    {
        get
        {
            EnsureLoaded();
            return _showHelpAndExit;
        }
    }

    public bool ShowVersionAndExit
    {
        get
        {
            EnsureLoaded();
            return _showVersionAndExit;
        }
    }

    public IEnumerable<ICodeSource> CodeSources
    {
        get
        {
            EnsureLoaded();
            return _codeSources;
        }
    }

    public IReadOnlyList<string> ScriptArguments
    {
        get
        {
            EnsureLoaded();
            return _scriptArguments;
        }
    }

    public IReadOnlyDictionary<string, string> ScriptProperties
    {
        get
        {
            EnsureLoaded();
            return _scriptProperties;
        }
    }

    public IEnumerable<string> NuGetSources
    {
        get
        {
            EnsureLoaded();
            return _nuGetSources;
        }
    }

    private void EnsureLoaded()
    {
        lock (_lockObject)
        {
            if (_isLoaded)
            {
                return;
            }

            _isLoaded = true;
            var defaultArgType = _runningMode switch
            {
                RunningMode.Tool => CommandLineArgumentType.ScriptFile,
                RunningMode.Application => CommandLineArgumentType.ScriptArgument,
                _ => CommandLineArgumentType.ScriptFile
            };

            var args = _commandLineParser.Parse(
                    _environment.GetCommandLineArgs().Skip(1),
                    defaultArgType)
                .ToImmutableArray();

            var props = new Dictionary<string, string>();
            _scriptProperties = props;
            foreach (var (_, value, key) in args.Where(i => i.ArgumentType == CommandLineArgumentType.ScriptProperty))
            {
                props[key] = value;
            }

            _nuGetSources = args.Where(i => i.ArgumentType == CommandLineArgumentType.NuGetSource).Select(i => i.Value);
            if (_runningMode == RunningMode.Application
                || args.Any(i => i.ArgumentType == CommandLineArgumentType.ScriptFile)
                || args.Any(i => i.ArgumentType == CommandLineArgumentType.Help))
            {
                _interactionMode = InteractionMode.NonInteractive;
                _verbosityLevel = VerbosityLevel.Normal;
                _showHelpAndExit = args.Any(i => i.ArgumentType == CommandLineArgumentType.Help);
                _showVersionAndExit = args.Any(i => i.ArgumentType == CommandLineArgumentType.Version);
                _scriptArguments = args.Where(i => i.ArgumentType == CommandLineArgumentType.ScriptArgument).Select(i => i.Value).ToImmutableArray();
                _codeSources = args.Where(i => i.ArgumentType == CommandLineArgumentType.ScriptFile).Select(i => _fileCodeSourceFactory(i.Value));
            }
            else
            {
                _interactionMode = InteractionMode.Interactive;
                _verbosityLevel = VerbosityLevel.Quiet;
                _codeSources = new[] {_consoleCodeSource};
            }
        }
    }

    VerbosityLevel ISettingSetter<VerbosityLevel>.SetSetting(VerbosityLevel value)
    {
        var prevVerbosityLevel = VerbosityLevel;
        _verbosityLevel = value;
        return prevVerbosityLevel;
    }
}