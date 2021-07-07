// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EmptyGeneralCatchClause
namespace Teamcity.Host
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    [ExcludeFromCodeCoverage]
    internal static class Disposable
    {
        public static readonly IDisposable Empty = EmptyDisposable.Shared;

        public static IDisposable Create(Action action)
        {
            return new DisposableAction(action);
        }

        public static IDisposable Create(params IDisposable[] disposables) =>
            Create((IEnumerable<IDisposable>) disposables);

        public static IDisposable Create(IEnumerable<IDisposable> disposables) => 
            new CompositeDisposable(disposables);

        private sealed class DisposableAction : IDisposable
        {
            private readonly Action _action;
            private readonly object? _key;
            private int _counter;

            public DisposableAction(Action action, object? key = null)
            {
                _action = action;
                _key = key ?? action;
            }

            public void Dispose()
            {
                if (Interlocked.Increment(ref _counter) != 1) return;
                try
                {
                    _action();
                }
                catch
                {
                }
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is DisposableAction other && Equals(_key, other._key);
            }

            public override int GetHashCode() => 
                _key != null ? _key.GetHashCode() : 0;
        }

        private sealed class CompositeDisposable : IDisposable
        {
            private readonly IEnumerable<IDisposable> _disposables;
            private int _counter;

            public CompositeDisposable(IEnumerable<IDisposable> disposables)
                => _disposables = disposables;

            public void Dispose()
            {
                if (Interlocked.Increment(ref _counter) != 1) return;
                foreach (var disposable in _disposables)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private sealed class EmptyDisposable : IDisposable
        {
            public static readonly IDisposable Shared = new EmptyDisposable();

            private EmptyDisposable() { }

            public void Dispose() { }
        }
    }
}
