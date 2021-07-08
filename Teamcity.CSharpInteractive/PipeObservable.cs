// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading;
    using System.Threading.Tasks;
    using Host;

    internal class PipeObservable: IObservable<string>, IActive
    {
        private readonly ILog<PipeObservable> _log;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ISession _session;
        private readonly List<IObserver<string>> _observers = new();

        public PipeObservable(
            ILog<PipeObservable> log,
            CancellationTokenSource cancellationTokenSource,
            ISession session)
        {
            _log = log;
            _cancellationTokenSource = cancellationTokenSource;
            _session = session;
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            lock (_observers) { _observers.Add(observer); }
            return Disposable.Create(() =>
            {
                lock (_observers) { _observers.Remove(observer); }
            });
        }

        public IDisposable Activate()
        {
            Run(_cancellationTokenSource.Token).ContinueWith(i => {});
            return Disposable.Create(() =>
            {
                _cancellationTokenSource.Dispose();
            });
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var value = await Receive(cancellationToken);
                    if (value == null)
                    {
                        continue;
                    }
                    
                    lock (_observers)
                    {
                        foreach (var observer in _observers)
                        {
                            observer.OnNext(value);
                        }
                    }
                }
                catch(Exception ex)
                {
                    _log.Error(ErrorId.Exception, new []{new Text(ex.Message)});
                }
            }
        }
        
        private async Task<string?> Receive(CancellationToken cancellationToken)
        {
            await using NamedPipeServerStream namedPipeServer = new(_session.Id, PipeDirection.InOut, 1);
            await namedPipeServer.WaitForConnectionAsync(cancellationToken);
            using var reader = new StreamReader(namedPipeServer);
            var line = await reader.ReadLineAsync();
            _log.Trace(new []{new Text($"Pipe {_session.Id} input: {line ?? "empty"}")});
            return line;
        }
    }
}