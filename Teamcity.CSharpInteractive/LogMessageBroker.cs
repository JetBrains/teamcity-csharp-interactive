// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using Host;

    internal class LogMessageBroker: IActive, IObserver<DtoError>, IObserver<DtoWarning>, IObserver<DtoInfo>, IObserver<DtoTrace>
    {
        private readonly ILog<LogMessageBroker> _log;
        private readonly IObservable<DtoError> _errorsSource;
        private readonly IObservable<DtoWarning> _warningsSource;
        private readonly IObservable<DtoInfo> _infoSource;
        private readonly IObservable<DtoTrace> _traceSource;

        public LogMessageBroker(
            IObservable<DtoError> errorsSource,
            IObservable<DtoWarning> warningsSource,
            IObservable<DtoInfo> infoSource,
            IObservable<DtoTrace> traceSource,
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
        
        void IObserver<DtoTrace>.OnNext(DtoTrace value) => 
            _log.Trace(new []{new Text(value.Trace)});

        void IObserver<DtoTrace>.OnError(Exception error) { }

        void IObserver<DtoTrace>.OnCompleted() { }

        void IObserver<DtoInfo>.OnNext(DtoInfo value) =>
            _log.Info(new []{new Text(value.Text)});

        void IObserver<DtoInfo>.OnError(Exception error) { }

        void IObserver<DtoInfo>.OnCompleted() { }
        
        void IObserver<DtoWarning>.OnNext(DtoWarning value) =>
            _log.Warning(new Text(value.Wraning));

        void IObserver<DtoWarning>.OnError(Exception error) { }

        void IObserver<DtoWarning>.OnCompleted() { }

        void IObserver<DtoError>.OnNext(DtoError value) =>
            _log.Error(new ErrorId(value.ErrorId), new []{new Text(value.Error)});

        void IObserver<DtoError>.OnError(Exception error) { }

        void IObserver<DtoError>.OnCompleted() { }
    }
}