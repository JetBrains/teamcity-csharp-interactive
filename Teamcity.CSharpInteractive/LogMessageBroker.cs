// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using Host;

    internal class LogMessageBroker: IActive, IObserver<ErrorContent>, IObserver<WarningContent>, IObserver<InfoContent>, IObserver<TraceContent>
    {
        private readonly ILog<LogMessageBroker> _log;
        private readonly IObservable<ErrorContent> _errorsSource;
        private readonly IObservable<WarningContent> _warningsSource;
        private readonly IObservable<InfoContent> _infoSource;
        private readonly IObservable<TraceContent> _traceSource;

        public LogMessageBroker(
            IObservable<ErrorContent> errorsSource,
            IObservable<WarningContent> warningsSource,
            IObservable<InfoContent> infoSource,
            IObservable<TraceContent> traceSource,
            ILog<LogMessageBroker> log)
        {
            _errorsSource = errorsSource;
            _warningsSource = warningsSource;
            _infoSource = infoSource;
            _traceSource = traceSource;
            _log = log;
        }

        public IDisposable Activate() =>
            Disposable.Create(
                _errorsSource.Subscribe(this),
                _warningsSource.Subscribe(this),
                _infoSource.Subscribe(this),
                _traceSource.Subscribe(this));
        
        void IObserver<TraceContent>.OnNext(TraceContent value) => 
            _log.Trace(new []{new Text(value.Trace)});

        void IObserver<TraceContent>.OnError(Exception error) { }

        void IObserver<TraceContent>.OnCompleted() { }

        void IObserver<InfoContent>.OnNext(InfoContent value) =>
            _log.Info(new []{new Text(value.Text)});

        void IObserver<InfoContent>.OnError(Exception error) { }

        void IObserver<InfoContent>.OnCompleted() { }
        
        void IObserver<WarningContent>.OnNext(WarningContent value) =>
            _log.Warning(new Text(value.Wraning));

        void IObserver<WarningContent>.OnError(Exception error) { }

        void IObserver<WarningContent>.OnCompleted() { }

        void IObserver<ErrorContent>.OnNext(ErrorContent value) =>
            _log.Error(new ErrorId(value.ErrorId), new []{new Text(value.Error)});

        void IObserver<ErrorContent>.OnError(Exception error) { }

        void IObserver<ErrorContent>.OnCompleted() { }
    }
}