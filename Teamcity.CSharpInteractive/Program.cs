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
        private readonly IExitTracker _exitTracker;
        private readonly Func<IRunner> _runner;

        internal Program(
            IInfo info,
            ISettingsManager settingsManager,
            IExitTracker exitTracker,
            Func<IRunner> runner)
        {
            _info = info;
            _settingsManager = settingsManager;
            _exitTracker = exitTracker;
            _runner = runner;
        }
        
        internal int Run()
        {
            _settingsManager.Load();
            using var exitToken = _exitTracker.Track();
            _info.ShowHeader();
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