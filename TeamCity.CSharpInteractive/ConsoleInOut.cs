namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

// ReSharper disable once ClassNeverInstantiated.Global
[ExcludeFromCodeCoverage]
internal class ConsoleInOut : IStdOut, IStdErr
{
    private readonly IConsole _console;
    private readonly ITextToColorStrings _textToColorStrings;
    private readonly IColorTheme _colorTheme;

    public ConsoleInOut(
        IConsole console,
        ITextToColorStrings textToColorStrings,
        IColorTheme colorTheme)
    {
        _console = console;
        _textToColorStrings = textToColorStrings;
        _colorTheme = colorTheme;
    }

    void IStdErr.WriteLine(params Text[] line) => WriteStdErr(line + Text.NewLine);

    public void Write(params Text[] text) => _console.WriteToOut(text.SelectMany(i => _textToColorStrings.Convert(i.Value, _colorTheme.GetConsoleColor(i.Color))).ToArray());

    void IStdOut.WriteLine(params Text[] line) => Write(line + Text.NewLine);

    private void WriteStdErr(params Text[] text) => _console.WriteToErr(text.Select(i => i.Value).ToArray());
}