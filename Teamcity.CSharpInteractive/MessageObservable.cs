namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class MessageObservable<T>: IObservable<T>, IObserver<string>
    {
        private readonly IObservable<string> _jsonSource;
        private readonly List<IObserver<T>> _observers = new();
        private IDisposable _subscriptionToken = Disposable.Empty;

        public MessageObservable(IObservable<string> jsonSource) => _jsonSource = jsonSource;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_observers)
            {
                _observers.Add(observer);
                if (_observers.Count == 1)
                {
                    _subscriptionToken.Dispose();
                    _subscriptionToken = _jsonSource.Subscribe(this);
                }
            }
            
            return Disposable.Create(() =>
            {
                lock (_observers)
                {
                    _observers.Remove(observer);
                    if (_observers.Count > 0)
                    {
                        return;
                    }

                    _subscriptionToken.Dispose();
                    _subscriptionToken = Disposable.Empty;
                }
            });
        }

        public void OnNext(string value)
        {
            var val = JsonConvert.DeserializeObject<T>(value);
            if (Equals(val, default(T)))
            {
                return;
            }
            
            lock (_observers)
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(val);
                }
            }
        }

        public void OnError(Exception error) { }

        public void OnCompleted() { }
    }
}