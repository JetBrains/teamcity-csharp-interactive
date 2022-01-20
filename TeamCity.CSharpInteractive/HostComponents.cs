namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using Host;

[ExcludeFromCodeCoverage]
internal readonly record struct HostComponents(
    IHost Host,
    IStatistics Statistics,
    IPresenter<Summary> SummaryPresenter,
    ILog<HostComponents> Log);