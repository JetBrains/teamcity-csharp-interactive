namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

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
                Color.Default => "0;37;40",
                Color.Header => "1;37",
                Color.Trace => "30;1",
                Color.Success => "32;1",
                Color.Warning => "31;1",
                Color.Error => "33;1",
                Color.Details => "36",
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
    }
}