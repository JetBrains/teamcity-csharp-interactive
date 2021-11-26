namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    // ReSharper disable once ClassNeverInstantiated.Global
    [ExcludeFromCodeCoverage]
    internal class ConsoleInOut : IStdOut, IStdErr
    {
        private readonly IConsole _console;
        private readonly ITextToColorStrings _textToColorStrings;
        private readonly IColorTheme _colorTheme;
        private readonly object _lockObject = new();

        public ConsoleInOut(
            IConsole console,
            ITextToColorStrings textToColorStrings,
            IColorTheme colorTheme)
        {
            _console = console;
            _textToColorStrings = textToColorStrings;
            _colorTheme = colorTheme;
        }

        void IStdErr.WriteLine(params Text[] line) => WriteStdErr(System.Console.Error, line + Text.NewLine);

        public void Write(params Text[] text)
        {
            foreach (var textItem in text)
            {
                _console.Write(_textToColorStrings.Convert(textItem.Value, _colorTheme.GetConsoleColor(textItem.Color)).ToArray());
            }
        }

        void IStdOut.WriteLine(params Text[] line) => Write(line + Text.NewLine);
        
        private void WriteStdErr(TextWriter textWriter, params Text[] text)
        {
            lock (_lockObject)
            {
                foreach (var textItem in text)
                {
                    textWriter.Write(textItem.Value);
                }
            }
        }
    }
}