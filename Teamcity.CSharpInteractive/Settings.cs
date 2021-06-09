// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class Settings : ISettings
    {
        private readonly IEnvironment _environment;
        private readonly ICodeSource _consoleCodeSource;
        private readonly IFileCodeSourceFactory _fileCodeSourceFactory;
        private IEnumerable<ICodeSource> _sources = Enumerable.Empty<ICodeSource>();

        public Settings(
            IEnvironment environment,
            ICodeSource consoleCodeSource,
            IFileCodeSourceFactory fileCodeSourceFactory)
        {
            _environment = environment;
            _consoleCodeSource = consoleCodeSource;
            _fileCodeSourceFactory = fileCodeSourceFactory;
        }

        public string Title => $"C# Interactive {Assembly.GetEntryAssembly()?.GetName().Version}";

        public VerbosityLevel VerbosityLevel { get; set; } = VerbosityLevel.Normal;

        public InteractionMode InteractionMode { get; private set; } = InteractionMode.Interactive;
        
        public IEnumerable<ICodeSource> Sources => _sources;

        public void Load()
        {
            var args = _environment.GetCommandLineArgs().ToArray();
            if (args.Length == 1)
            {
                InteractionMode = InteractionMode.Interactive;
                VerbosityLevel = VerbosityLevel.Quit;
                _sources = new []{_consoleCodeSource};
            }
            else
            {
                InteractionMode = InteractionMode.Script;
                VerbosityLevel = VerbosityLevel.Normal;
                _sources = args.Skip(1).Select(i => _fileCodeSourceFactory.Create(i));
            }
        }
    }
}