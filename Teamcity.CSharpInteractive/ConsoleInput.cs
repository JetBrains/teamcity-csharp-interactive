// ReSharper disable ClassNeverInstantiated.Global

namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    internal class ConsoleInput : ICodeSource
    {
        public string Name => "Console";
        
        public IEnumerator<string> GetEnumerator() => new LineEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class LineEnumerator : IEnumerator<string>
        {
            private readonly CancellationTokenSource _cancellationTokenSource = new();

            public LineEnumerator() => Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            public string Current { get; private set; } = string.Empty;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                try
                {
                    var cancellationToken = _cancellationTokenSource.Token;
                    var task = Task.Run(() => Console.In.ReadLine(), cancellationToken);
                    task.Wait(cancellationToken);
                    Current = task.Result ?? string.Empty;
                    return Current != string.Empty;
                }
                catch(OperationCanceledException)
                {
                    return false;
                }
            }

            public void Reset() { }

            public void Dispose()
            {
                Console.CancelKeyPress -= ConsoleOnCancelKeyPress;
                _cancellationTokenSource.Dispose();
            }

            private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e) => _cancellationTokenSource.Cancel();
        }
    }
}