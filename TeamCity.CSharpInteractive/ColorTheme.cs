namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Contracts;

    // ReSharper disable once ClassNeverInstantiated.Global
    [ExcludeFromCodeCoverage]
    internal class ColorTheme : IColorTheme
    {
        public ConsoleColor GetConsoleColor(Color color) =>
            color switch
            {
                Color.Default => ConsoleColor.Gray,
                Color.Header => ConsoleColor.White,
                Color.Trace => ConsoleColor.DarkGray,
                Color.Success => ConsoleColor.Green,
                Color.Warning => ConsoleColor.Yellow,
                Color.Error => ConsoleColor.Red,
                Color.Details => ConsoleColor.DarkCyan,
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };

        public string GetAnsiColor(Color color) =>
            color switch
            {
                Color.Default => "39",
                Color.Header => "39;1",
                Color.Trace => "90",
                Color.Success => "32;1",
                Color.Warning => "33",
                Color.Error => "31",
                Color.Details => "36",
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
    }
}