namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    // ReSharper disable once ClassNeverInstantiated.Global
    [ExcludeFromCodeCoverage]
    internal class ConsoleOutput : IStdOut, IStdErr
    {
        private readonly IColorTheme _colorTheme;

        public ConsoleOutput(IColorTheme colorTheme) =>
            _colorTheme = colorTheme ?? throw new ArgumentNullException(nameof(colorTheme));

        void IStdErr.Write(params Text[] text) =>
            WriteInternal(Console.Error, text);

        void IStdOut.Write(params Text[] text) =>
            WriteInternal(Console.Out, text);

        private void WriteInternal(TextWriter textWriter, params Text[] text)
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