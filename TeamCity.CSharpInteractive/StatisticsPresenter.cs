// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using Contracts;

    [ExcludeFromCodeCoverage]
    internal class StatisticsPresenter : IPresenter<IStatistics>
    {
        private readonly ILog<StatisticsPresenter> _log;
        private readonly IStringService _stringService;

        public StatisticsPresenter(ILog<StatisticsPresenter> log, IStringService stringService)
        {
            _log = log;
            _stringService = stringService;
        }

        public void Show(IStatistics statistics)
        {
            foreach (var error in statistics.Errors)
            {
                _log.Info(new []{ new Text(error, Color.Error)});
            }
            
            foreach (var warning in statistics.Warnings)
            {
                _log.Info(new []{ new Text(warning, Color.Warning)});
            }

            if (statistics.Warnings.Count > 0)
            {
                _log.Info(new []{new Text($"{_stringService.Tab}{statistics.Warnings.Count} Warning(s)")});
            }
            
            if (statistics.Errors.Count > 0)
            {
                _log.Info(new []{new Text($"{_stringService.Tab}{statistics.Errors.Count} Error(s)", Color.Error)});
            }
            
            _log.Info(new Text($"Time Elapsed {statistics.TimeElapsed:g}"));
        }
    }
}