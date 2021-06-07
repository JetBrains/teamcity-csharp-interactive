// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;

    internal class TracePresenter: IPresenter<IEnumerable<ITraceSource>>
    {
        private readonly ILog<TracePresenter> _log;
        
        public TracePresenter(ILog<TracePresenter> log) =>
            _log = log;

        public void Show(IEnumerable<ITraceSource> data) =>
            _log.Trace(data.SelectMany(i => i.GetTrace()).ToArray());
    }
}