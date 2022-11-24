// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using HostApi;

[ExcludeFromCodeCoverage]
internal class AnsiInOut : IStdOut, IStdErr
{
    public const char EscapeSymbol = '\x001B';
    private readonly object _lockObject = new();
    private readonly IColorTheme _colorTheme;

    public AnsiInOut(IColorTheme colorTheme) => _colorTheme = colorTheme;

    public void Write(params Text[] text) => 
        Write(System.Console.Out, text, Color.Default);

    void IStdOut.WriteLine(params Text[] line) => 
        Write(System.Console.Out, line + Text.NewLine, Color.Default);

    public void WriteLine(params Text[] errorLine) => 
        Write(System.Console.Error, errorLine + Text.NewLine, Color.Error);

    private void Write(TextWriter writer, IEnumerable<Text> text, Color defaultColor)
    {
        lock (_lockObject)
        {
            foreach (var item in text)
            {
                Write(writer, item, defaultColor);
            }
        }
    }

    private void Write(TextWriter writer, Text text, Color defaultColor)
    {
        var color = text.Color == Color.Default ? defaultColor : text.Color;
        writer.Write(
            color == Color.Default
                ? text.Value
                : $"{EscapeSymbol}[{_colorTheme.GetAnsiColor(color)}m{text.Value}{EscapeSymbol}[{_colorTheme.GetAnsiColor(Color.Default)}m");
    }
}