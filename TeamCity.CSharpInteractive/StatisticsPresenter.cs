// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using Contracts;

    [ExcludeFromCodeCoverage]
    internal class StatisticsPresenter : IPresenter<IStatistics>
    {
        private readonly ILog<StatisticsPresenter> _log;

        public StatisticsPresenter(ILog<StatisticsPresenter> log) => _log = log;

        public void Show(IStatistics statistics)
        {
            foreach (var error in statistics.Errors)
            {
                _log.Info(Text.Tab, new Text(error, Color.Error));
            }
            
            foreach (var warning in statistics.Warnings)
            {
                _log.Info(Text.Tab, new Text(warning, Color.Warning));
            }

            if (statistics.Warnings.Count > 0)
            {
                _log.Info(new Text($"{statistics.Warnings.Count} Warning(s)"));
            }
            
            if (statistics.Errors.Count > 0)
            {
                _log.Info(new Text($"{statistics.Errors.Count} Error(s)", Color.Error));
            }
            
            _log.Info(new Text($"Time Elapsed {statistics.TimeElapsed:g}"));
        }
    }
}