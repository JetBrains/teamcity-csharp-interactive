// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal class TracePresenter: IPresenter<IEnumerable<ITraceSource>>
    {
        private readonly ILog<TracePresenter> _log;
        
        public TracePresenter(ILog<TracePresenter> log) =>
            _log = log;

        public void Show(IEnumerable<ITraceSource> data)
        {
            foreach (var source in data)
            foreach (var text in source.GetTrace())
            {
                _log.Trace(() => new []{text});
            }
        }
    }
}