// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class TracePresenter: IPresenter<IEnumerable<ITraceSource>>
{
    private readonly ILog<TracePresenter> _log;
        
    public TracePresenter(ILog<TracePresenter> log) =>
        _log = log;

    public void Show(IEnumerable<ITraceSource> data) =>
        _log.Trace(() => ( 
            from source in data
            from text in source.Trace
            select text).ToArray());
}