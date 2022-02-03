namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using HostApi;

// ReSharper disable once ClassNeverInstantiated.Global
[ExcludeFromCodeCoverage]
internal class ColorTheme : IColorTheme
{
    public ConsoleColor GetConsoleColor(Color color)
    {
        if (System.Console.BackgroundColor != ConsoleColor.White)
        {
            return color switch
            {
                Color.Default => ConsoleColor.Gray,
                Color.Header => ConsoleColor.White,
                Color.Trace => ConsoleColor.DarkGray,
                Color.Success => ConsoleColor.Green,
                Color.Warning => ConsoleColor.Yellow,
                Color.Error => ConsoleColor.Red,
                Color.Details => ConsoleColor.DarkBlue,
                Color.Highlighted => ConsoleColor.DarkCyan,
                _ => ConsoleColor.Gray
            };
        }

        return color switch
        {
            Color.Default => ConsoleColor.Black,
            Color.Header => ConsoleColor.Black,
            Color.Trace => ConsoleColor.DarkGray,
            Color.Success => ConsoleColor.Green,
            Color.Warning => ConsoleColor.Yellow,
            Color.Error => ConsoleColor.Red,
            Color.Details => ConsoleColor.DarkBlue,
            Color.Highlighted => ConsoleColor.DarkCyan,
            _ => ConsoleColor.Black
        };
    }

    public string GetAnsiColor(Color color) =>
        color switch
        {
            Color.Default => "39",
            Color.Header => "39;1",
            Color.Trace => "90",
            Color.Success => "32;1",
            Color.Warning => "33",
            Color.Error => "31",
            Color.Details => "34;1",
            Color.Highlighted => "36",
            _ => "39"
        };
}