namespace TeamCity.CSharpInteractive;

using System.Collections;

internal class LinesEnumerator : IEnumerator<string>
{
    private readonly IEnumerator<string> _baseEnumerator;
    private readonly Action _onDispose;

    public LinesEnumerator(IEnumerable<string> enumerable, Action onDispose)
    {
        _baseEnumerator = enumerable.GetEnumerator();
        _onDispose = onDispose;
    }

    public bool MoveNext() => _baseEnumerator.MoveNext();

    public void Reset() => _baseEnumerator.Reset();

    public string Current => _baseEnumerator.Current;

    object? IEnumerator.Current => ((IEnumerator)_baseEnumerator).Current;

    public void Dispose()
    {
        try
        {
            _baseEnumerator.Dispose();
        }
        finally
        {
            _onDispose();
        }
    }
}