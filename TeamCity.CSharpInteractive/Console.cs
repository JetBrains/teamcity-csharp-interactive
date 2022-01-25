namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using HostApi;

// ReSharper disable once ClassNeverInstantiated.Global
[ExcludeFromCodeCoverage]
internal class Console : IConsole
{
    private readonly object _lockObject = new();
    private readonly ConsoleColor _errorColor;

    public Console(IColorTheme colorTheme) => 
        _errorColor = colorTheme.GetConsoleColor(Color.Error);

    public void WriteToOut(params (ConsoleColor? color, string output)[] text)
    {
        lock (_lockObject)
        {
            var foregroundColor = System.Console.ForegroundColor;
            try
            {
                foreach (var (color, output) in text)
                {
                    if (color.HasValue)
                    {
                        System.Console.ForegroundColor = color.Value;
                    }

                    System.Console.Out.Write(output);
                }
            }
            finally
            {
                System.Console.ForegroundColor = foregroundColor;
            }
        }
    }

    public void WriteToErr(params string[] text)
    {
        lock (_lockObject)
        {
            var foregroundColor = System.Console.ForegroundColor;
            try
            {
                System.Console.ForegroundColor = _errorColor;
                foreach (var item in text)
                {
                    System.Console.Error.Write(item);
                }
            }
            finally
            {
                System.Console.ForegroundColor = foregroundColor;
            }
        }
    }
}