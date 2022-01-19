namespace TeamCity.CSharpInteractive;

using Contracts;

internal readonly record struct HostComponents(
    IHost Host,
    IStatistics Statistics,
    IPresenter<IStatistics> StatisticsPresenter,
    ILog<HostComponents> Log);