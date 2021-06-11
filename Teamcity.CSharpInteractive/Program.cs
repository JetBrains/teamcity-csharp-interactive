// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;

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
        private readonly ISettings _settings;
        private readonly IExitTracker _exitTracker;
        private readonly IEnumerable<IRunner> _runners;

        internal Program(
            IInfo info,
            ISettings settings,
            IExitTracker exitTracker,
            IEnumerable<IRunner> runners)
        {
            _info = info;
            _settings = settings;
            _exitTracker = exitTracker;
            _runners = runners;
        }
        
        internal int Run()
        {
            _settings.Load();

            using var exitToken = _exitTracker.Track();
            _info.ShowHeader();
            try
            {
                var runner = _runners.Single(i => i.InteractionMode == _settings.InteractionMode);
                return (int) runner.Run();
            }
            finally
            {
                _info.ShowFooter();
            }
        }
    }
}