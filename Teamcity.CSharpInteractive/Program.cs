// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System;

    public class Program
    {
        public static int Main()
        {
            try
            {
                return Composer.Resolve<Program>().Run();
            }
            finally
            {
                Composer.FinalDispose();
            }
        }

        private readonly IInfo _info;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettings _settings;
        private readonly IExitTracker _exitTracker;
        private readonly Func<IRunner> _runner;

        internal Program(
            IInfo info,
            ISettingsManager settingsManager,
            ISettings settings,
            IExitTracker exitTracker,
            Func<IRunner> runner)
        {
            _info = info;
            _settingsManager = settingsManager;
            _settings = settings;
            _exitTracker = exitTracker;
            _runner = runner;
        }
        
        internal int Run()
        {
            _settingsManager.Load();
            if (_settings.ShowVersionAndExit)
            {
                _info.ShowVersion();
                return 0;
            }

            _info.ShowHeader();

            if (_settings.ShowHelpAndExit)
            {
                _info.ShowHelp();
                return 0;
            }
            
            using var exitToken = _exitTracker.Track();
            try
            {
                return (int)_runner().Run();
            }
            finally
            {
                _info.ShowFooter();
            }
        }
    }
}