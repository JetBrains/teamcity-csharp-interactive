// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class SummaryPresenter : IPresenter<Summary>
{
    private readonly ILog<SummaryPresenter> _log;
    private readonly IStatistics _statistics;
    private readonly IPresenter<IStatistics> _statisticsPresenter;

    public SummaryPresenter(
        ILog<SummaryPresenter> log,
        IStatistics statistics,
        IPresenter<IStatistics> statisticsPresenter)
    {
        _log = log;
        _statistics = statistics;
        _statisticsPresenter = statisticsPresenter;
    }

    public void Show(Summary summary)
    {
        _statisticsPresenter.Show(_statistics);
        var state = summary.Success == false || _statistics.Errors.Any()
            ? new Text("Running FAILED.", Color.Error)
            : new Text("Running succeeded.", Color.Success);
        _log.Info(state);
    }
}