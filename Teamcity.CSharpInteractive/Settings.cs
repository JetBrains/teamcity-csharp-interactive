// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;
    using Pure.DI;

    internal class Settings : ISettings, ISettingsManager
    {
        private readonly IEnvironment _environment;
        private readonly ICommandLineParser _commandLineParser;
        private readonly ICodeSource _hostCodeSource;
        private readonly ICodeSource _consoleCodeSource;
        private readonly IInitialStateCodeSourceFactory _initialStateCodeSourceFactory;
        private readonly IFileCodeSourceFactory _fileCodeSourceFactory;

        public Settings(
            IEnvironment environment,
            ICommandLineParser commandLineParser,
            [Tag("Host")] ICodeSource hostCodeSource,
            ICodeSource consoleCodeSource,
            IInitialStateCodeSourceFactory initialStateCodeSourceFactory,
            IFileCodeSourceFactory fileCodeSourceFactory)
        {
            _environment = environment;
            _commandLineParser = commandLineParser;
            _hostCodeSource = hostCodeSource;
            _consoleCodeSource = consoleCodeSource;
            _initialStateCodeSourceFactory = initialStateCodeSourceFactory;
            _fileCodeSourceFactory = fileCodeSourceFactory;
        }

        public VerbosityLevel VerbosityLevel { get; set; } = VerbosityLevel.Normal;

        public InteractionMode InteractionMode { get; private set; } = InteractionMode.Interactive;

        public bool ShowHelpAndExit { get; private set; }

        public bool ShowVersionAndExit { get; private set; }

        public IEnumerable<ICodeSource> CodeSources { get; private set; } = Enumerable.Empty<ICodeSource>();
        
        public IEnumerable<string> ScriptArguments { get; private set; }  = Enumerable.Empty<string>();

        public IReadOnlyDictionary<string, string> ScriptProperties { get; private set; } = new Dictionary<string, string>();

        public IEnumerable<string> NuGetSources { get; private set; }  = Enumerable.Empty<string>();

        public void Load()
        {
            var args = _commandLineParser.Parse(_environment.GetCommandLineArgs().Skip(1)).ToArray();
            if (args.Length == 0)
            {
                InteractionMode = InteractionMode.Interactive;
                VerbosityLevel = VerbosityLevel.Quiet;
                CodeSources = new []{ _hostCodeSource, _consoleCodeSource };
            }
            else
            {
                InteractionMode = InteractionMode.Script;
                VerbosityLevel = VerbosityLevel.Normal;
                ShowHelpAndExit = args.Any(i => i.ArgumentType == CommandLineArgumentType.Help);
                ShowVersionAndExit = args.Any(i => i.ArgumentType == CommandLineArgumentType.Version);
                ScriptArguments = args.Where(i => i.ArgumentType == CommandLineArgumentType.ScriptArgument).Select(i => i.Value);
                var props = new Dictionary<string, string>();
                ScriptProperties = props;
                foreach (var prop in args.Where(i => i.ArgumentType == CommandLineArgumentType.ScriptProperty))
                {
                    props[prop.Key] = prop.Value;
                }

                NuGetSources = args.Where(i => i.ArgumentType == CommandLineArgumentType.NuGetSource).Select(i => i.Value);
                CodeSources =
                    new [] {_hostCodeSource, _initialStateCodeSourceFactory.Create(ScriptArguments.ToArray(), ScriptProperties)}.Concat(
                        args.Where(i => i.ArgumentType == CommandLineArgumentType.ScriptFile).Select(i => _fileCodeSourceFactory.Create(i.Value)));
            }
        }
    }
}