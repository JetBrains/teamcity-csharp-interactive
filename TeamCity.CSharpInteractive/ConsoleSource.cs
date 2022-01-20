// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class ConsoleSource : ICodeSource, IEnumerator<string?>
{
    private readonly CancellationToken _cancellationToken;

    public ConsoleSource(CancellationToken cancellationToken) => _cancellationToken = cancellationToken;

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
            Task.Run(() => { Current = System.Console.In.ReadLine() ?? string.Empty; }, _cancellationToken).Wait(_cancellationToken);
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