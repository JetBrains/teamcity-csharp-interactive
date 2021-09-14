// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    internal class ConsoleInput : ICodeSource, IEnumerator<string?>
    {
        public ConsoleInput() => Console.CancelKeyPress += (_, _) => System.Environment.Exit(0);

        public string Name => "Console";

        public bool Internal => false;

        public IEnumerator<string?> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public string? Current { get; private set; } = string.Empty;

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (Current == default)
            {
                Task.Run(() => { Current = Console.In.ReadLine() ?? string.Empty; }).Wait();
            }
            else
            {
                Current = null;
            }
            
            return true;
        }

        public void Reset() { }

        public void Dispose() { }
    }
}