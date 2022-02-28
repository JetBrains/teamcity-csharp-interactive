namespace TeamCity.CSharpInteractive;

using System.Collections;

internal class LineCodeSource: ICodeSource
{
    public string Line { get; set; } = string.Empty;

    public IEnumerator<string?> GetEnumerator() => Enumerable.Repeat(Line, 1).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public string Name => Line;

    public bool Internal => true;
}