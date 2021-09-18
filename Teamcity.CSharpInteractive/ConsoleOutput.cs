namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    // ReSharper disable once ClassNeverInstantiated.Global
    [ExcludeFromCodeCoverage]
    internal class ConsoleOutput : IStdOut, IStdErr
    {
        private readonly IColorTheme _colorTheme;
        private readonly object _lockObject = new();

        public ConsoleOutput(IColorTheme colorTheme) =>
            _colorTheme = colorTheme ?? throw new ArgumentNullException(nameof(colorTheme));

        void IStdErr.WriteLine(params Text[] line) => WriteText(Console.Error, line + Text.NewLine);

        public void Write(params Text[] text) => WriteText(Console.Out, text);

        void IStdOut.WriteLine(params Text[] line) => WriteText(Console.Out, line + Text.NewLine);

        private void WriteText(TextWriter textWriter, params Text[] text)
        {
            lock (_lockObject)
            {
                var foregroundColor = Console.ForegroundColor;
                try
                {
                    foreach (var textItem in text)
                    {
                        Console.ForegroundColor = _colorTheme.GetConsoleColor(textItem.Color);
                        textWriter.Write(textItem.Value);
                    }
                }
                finally
                {
                    Console.ForegroundColor = foregroundColor;
                }
            }
        }
    }
}