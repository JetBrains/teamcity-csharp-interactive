// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Program
    {
        public static int Main()
        {
            try
            {
                return (int)Composer.Resolve<Program>().Run();
            }
            finally
            {
                Composer.FinalDispose();
            }
        }

        private readonly ILog<Program> _log;
        private readonly IEnumerable<IActive> _activeObjects;
        private readonly IInfo _info;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettings _settings;
        private readonly IExitTracker _exitTracker;
        private readonly Func<IRunner> _runner;

        internal Program(
            ILog<Program> log,
            IEnumerable<IActive> activeObjects,
            IInfo info,
            ISettingsManager settingsManager,
            ISettings settings,
            IExitTracker exitTracker,
            Func<IRunner> runner)
        {
            _log = log;
            _activeObjects = activeObjects;
            _info = info;
            _settingsManager = settingsManager;
            _settings = settings;
            _exitTracker = exitTracker;
            _runner = runner;
        }
        
        internal ExitCode Run()
        {
            _settingsManager.Load();
            if (_settings.ShowVersionAndExit)
            {
                _info.ShowVersion();
                return ExitCode.Success;
            }

            _info.ShowHeader();

            if (_settings.ShowHelpAndExit)
            {
                _info.ShowHelp();
                return ExitCode.Success;
            }
            
            using var exitToken = _exitTracker.Track();
            try
            {
                using (Disposable.Create(_activeObjects.Select(i => i.Activate()).ToArray()))
                {
                    return _runner().Run();
                }
            }
            catch(Exception error)
            {
                _log.Error(ErrorId.Unhandled, error);
                return ExitCode.Fail;
            }
            finally
            {
                _info.ShowFooter();
            }
        }
    }
}