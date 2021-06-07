// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;

    public class Program
    {
        public static int Main() => Composer.Resolve<Program>().Run();

        private readonly IInfo _info;
        private readonly ISettings _settings;
        private readonly IEnumerable<IRunner> _runners;

        internal Program(
            IInfo info,
            ISettings settings,
            IEnumerable<IRunner> runners)
        {
            _info = info;
            _settings = settings;
            _runners = runners;
        }
        
        internal int Run()
        {
            _settings.Load();
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